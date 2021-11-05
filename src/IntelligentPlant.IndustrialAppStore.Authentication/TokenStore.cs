using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.DependencyInjection;

namespace IntelligentPlant.IndustrialAppStore.Authentication {

    /// <summary>
    /// Base class for <see cref="ITokenStore"/> implementations.
    /// </summary>
    public abstract class TokenStore : ITokenStore {

        /// <summary>
        /// The authentication options.
        /// </summary>
        private readonly IndustrialAppStoreAuthenticationOptions _options;

        /// <summary>
        /// The backchannel HTTP client to use.
        /// </summary>
        private readonly HttpClient _backchannelHttpClient;

        /// <summary>
        /// The system clock.
        /// </summary>
        internal ISystemClock Clock { get; }


        /// <summary>
        /// Creates a new <see cref="TokenStore"/> object.
        /// </summary>
        /// <param name="options">
        ///   The authentication options.
        /// </param>
        /// <param name="httpClient">
        ///   The backchannel HTTP client to use.
        /// </param>
        /// <param name="clock">
        ///   The system clock.
        /// </param>
        protected TokenStore(
            IndustrialAppStoreAuthenticationOptions options,
            HttpClient httpClient,
            ISystemClock clock
        ) {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _backchannelHttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            Clock = clock ?? throw new ArgumentNullException(nameof(clock));
        }


        /// <summary>
        /// Initialises the token store for the specified user session.
        /// </summary>
        /// <param name="userId">
        ///   The user ID.
        /// </param>
        /// <param name="sessionId">
        ///   The session ID.
        /// </param>
        /// <param name="properties">
        ///   The authentication properties for the user session.
        /// </param>
        /// <returns>
        ///   A <see cref="ValueTask"/> that will initialise the token store.
        /// </returns>
        protected internal abstract ValueTask InitAsync(string userId, string sessionId, AuthenticationProperties properties);


        /// <inheritdoc/>
        async ValueTask<OAuthTokens?> ITokenStore.GetTokensAsync() {
            var oauthTokens = await GetTokensAsync();
            
            if (string.IsNullOrWhiteSpace(oauthTokens?.AccessToken)) {
                return null;
            }

            if (oauthTokens.Value.UtcExpiresAt == null) {
                // No expiry specified (this would be unusual).
                return oauthTokens;
            }

            if (oauthTokens.Value.UtcExpiresAt.Value > DateTime.UtcNow) {
                // Access token is still valid.
                return oauthTokens;
            }

            // Access token has expired. If we have a refresh token, we will try and use it.
            
            if (string.IsNullOrWhiteSpace(oauthTokens.Value.RefreshToken)) {
                // No refresh token available.
                return null;
            }

            var tokensResponse = await UseRefreshTokenAsync(oauthTokens.Value.RefreshToken);

            DateTimeOffset? utcExpiresAt = null;

            if (!string.IsNullOrEmpty(tokensResponse.ExpiresIn)) {
                if (int.TryParse(tokensResponse.ExpiresIn, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)) {
                    utcExpiresAt = Clock.UtcNow.AddSeconds(value);
                }
            }

            var tokens = new OAuthTokens(tokensResponse.TokenType, tokensResponse.AccessToken, tokensResponse.RefreshToken, utcExpiresAt);
            await SaveTokensAsync(tokens);

            return tokens;
        }


        /// <inheritdoc/>
        async ValueTask ITokenStore.SaveTokensAsync(OAuthTokens tokens) {
            await SaveTokensAsync(tokens);
        }


        /// <summary>
        /// Exchanges a refresh token for a new set of tokens.
        /// </summary>
        /// <param name="refreshToken">
        ///   The refresh token.
        /// </param>
        /// <returns>
        ///   The OAuth token response.
        /// </returns>
        private async Task<OAuthTokenResponse> UseRefreshTokenAsync(string refreshToken) {
            var tokenRequestParameters = new Dictionary<string, string>() {
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = refreshToken
            };

#if NETCOREAPP
            tokenRequestParameters["client_id"] = _options.ClientId;
            if (!string.IsNullOrWhiteSpace(_options.ClientSecret) && !string.Equals(_options.ClientSecret, IndustrialAppStoreAuthenticationExtensions.DefaultClientSecret, StringComparison.OrdinalIgnoreCase)) {
                tokenRequestParameters["client_secret"] = _options.ClientSecret;
            }
#else
            tokenRequestParameters["client_id"] = _options.ClientId;
            tokenRequestParameters["client_secret"] = _options.ClientSecret;
#endif

            var refreshRequestContent = new FormUrlEncodedContent(tokenRequestParameters);
            var refreshRequest = new HttpRequestMessage(HttpMethod.Post, _options.GetTokenEndpoint()) {
                Content = refreshRequestContent
            };
            refreshRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var refreshResponse = await _backchannelHttpClient.SendAsync(refreshRequest, default);

#if NETCOREAPP
            var tokenResponseJson = System.Text.Json.JsonDocument.Parse(await refreshRequest.Content.ReadAsStreamAsync());
#else
            var tokenResponseJson = Newtonsoft.Json.Linq.JObject.Parse(await refreshResponse.Content.ReadAsStringAsync());
#endif

            return OAuthTokenResponse.Success(tokenResponseJson);
        }


        /// <summary>
        /// Gets the OAuth tokens for the current context.
        /// </summary>
        /// <returns>
        ///   A <see cref="Task{TResult}"/> that will return the associated <see cref="OAuthTokens"/> 
        ///   for the identity.
        /// </returns>
        protected abstract Task<OAuthTokens?> GetTokensAsync();


        /// <summary>
        /// Saves the OAuth tokens for the current context.
        /// </summary>
        /// <param name="tokens">
        ///   The tokens.
        /// </param>
        /// <returns>
        ///   A <see cref="Task"/> that will save the tokens.
        /// </returns>
        protected internal abstract Task SaveTokensAsync(OAuthTokens tokens);

    }
}
