using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
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
        /// Flags if <see cref="ITokenStore.InitAsync"/> has been called.
        /// </summary>
        private int _initialised;

        /// <summary>
        /// The system clock.
        /// </summary>
        internal ISystemClock Clock { get; }

        /// <summary>
        /// The user ID for the token store.
        /// </summary>
        protected string UserId { get; private set; }

        /// <summary>
        /// The session ID for the token store.
        /// </summary>
        protected string SessionId { get; private set; }

        /// <summary>
        /// The <see cref="Microsoft.AspNetCore.Authentication.AuthenticationProperties"/> for the 
        /// token store.
        /// </summary>
        protected AuthenticationProperties AuthenticationProperties { get; private set; }


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


        async ValueTask ITokenStore.InitAsync(string userId, string sessionId, AuthenticationProperties properties) {
            if (userId == null) {
                throw new ArgumentNullException(nameof(userId));
            }
            if (sessionId == null) {
                throw new ArgumentNullException(nameof(sessionId));
            }
            if (properties == null) {
                throw new ArgumentNullException(nameof(properties));
            }
            if (Interlocked.CompareExchange(ref _initialised, 1, 0) != 0) {
                throw new InvalidOperationException(Resources.Error_TokenStoreHasAlreadyBeenInitialised);
            }

            UserId = userId;
            SessionId = sessionId;
            AuthenticationProperties = properties;

            await InitAsync();
        }


        /// <inheritdoc/>
        async ValueTask<OAuthTokens?> ITokenStore.GetTokensAsync() {
            if (_initialised == 0) {
                throw new InvalidOperationException(Resources.Error_TokenStoreHasNotBeenInitialised);
            }

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
            if (_initialised == 0) {
                throw new InvalidOperationException(Resources.Error_TokenStoreHasNotBeenInitialised);
            }
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
        /// Initialises the token store for the configured user session.
        /// </summary>
        /// <returns>
        ///   A <see cref="ValueTask"/> that will initialise the token store.
        /// </returns>
        protected abstract ValueTask InitAsync();


        /// <summary>
        /// Gets the tokens associated with the authenticated user.
        /// </summary>
        /// <returns>
        ///   A <see cref="ValueTask{TResult}"/> that will return either the tokens for the 
        ///   authenticated user, or <see langword="null"/> if the access token is unavailable or 
        ///   has expired.
        /// </returns>
        protected abstract ValueTask<OAuthTokens?> GetTokensAsync();


        /// <summary>
        /// Saves tokens associated with the authenticated user.
        /// </summary>
        /// <param name="tokens">
        ///   The tokens to save.
        /// </param>
        /// <returns>
        ///   A <see cref="ValueTask"/> that will save the tokens.
        /// </returns>
        protected internal abstract ValueTask SaveTokensAsync(OAuthTokens tokens);

    }
}
