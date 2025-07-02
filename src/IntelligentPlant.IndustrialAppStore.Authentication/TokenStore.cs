using System.Globalization;
using System.Net.Http.Headers;

using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IntelligentPlant.IndustrialAppStore.Authentication {

    /// <summary>
    /// Base class for <see cref="ITokenStore"/> implementations.
    /// </summary>
    public abstract partial class TokenStore : ITokenStore {

        /// <summary>
        /// The logger for the token store.
        /// </summary>
        private readonly ILogger _logger;

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
        private readonly TimeProvider _timeProvider;

        /// <summary>
        /// Flags if <see cref="ITokenStore.InitAsync"/> has been called.
        /// </summary>
        private int _initialised;

        /// <inheritdoc/>
        public bool Ready => _initialised == 1;

        /// <summary>
        /// The user ID for the token store.
        /// </summary>
        protected string? UserId { get; private set; }

        /// <summary>
        /// The session ID for the token store.
        /// </summary>
        protected string? SessionId { get; private set; }


        /// <summary>
        /// Creates a new <see cref="TokenStore"/> object.
        /// </summary>
        /// <param name="options">
        ///   The authentication options.
        /// </param>
        /// <param name="httpClient">
        ///   The backchannel HTTP client to use.
        /// </param>
        /// <param name="timeProvider">
        ///   The time provider.
        /// </param>
        /// <param name="logger">
        ///   The logger for the token store.
        /// </param>
        protected TokenStore(
            IOptions<IndustrialAppStoreAuthenticationOptions> options,
            HttpClient httpClient,
            TimeProvider timeProvider,
            ILogger<TokenStore>? logger = null
        ) {
            _options = options.Value;
            _backchannelHttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
            _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<TokenStore>.Instance;
        }


        /// <inheritdoc/>
        async ValueTask ITokenStore.InitAsync(string userId, string sessionId) {
            await InitCoreAsync(userId, sessionId).ConfigureAwait(false);
        }


        /// <summary>
        /// Initialises the token store.
        /// </summary>
        /// <param name="userId">
        ///   The user ID for the token store.
        /// </param>
        /// <param name="sessionId">
        ///   The session ID for the token store.
        /// </param>
        /// <returns>
        ///   A <see cref="ValueTask"/> that will initialise the token store.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="userId"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="sessionId"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///   The token store has already been initialised.
        /// </exception>
        protected async ValueTask InitCoreAsync(string userId, string sessionId) {
            if (userId == null) {
                throw new ArgumentNullException(nameof(userId));
            }
            if (sessionId == null) {
                throw new ArgumentNullException(nameof(sessionId));
            }
            if (Interlocked.CompareExchange(ref _initialised, 1, 0) != 0) {
                throw new InvalidOperationException(Resources.Error_TokenStoreHasAlreadyBeenInitialised);
            }

            UserId = userId;
            SessionId = sessionId;

            await InitAsync();
        }


        /// <inheritdoc/>
        async ValueTask<OAuthTokens?> ITokenStore.GetTokensAsync() {
            if (!Ready) {
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

            // If we have a refresh token, we will assume that the access token has expired 30
            // seconds before it actually does. This is designed to prevent last-minute expiry
            // issues caused by e.g. clocks being slightly out of sync.
            var tokenExpiryComparisonTime = string.IsNullOrWhiteSpace(oauthTokens.Value.RefreshToken)
                ? _timeProvider.GetUtcNow()
                : _timeProvider.GetUtcNow().AddSeconds(30);

            if (oauthTokens.Value.UtcExpiresAt.Value > tokenExpiryComparisonTime) {
                // Access token is still valid.
                return oauthTokens;
            }

            // Access token has expired. If we have a refresh token, we will try and use it.
            
            if (string.IsNullOrWhiteSpace(oauthTokens.Value.RefreshToken)) {
                // No refresh token available.
                return null;
            }

            OAuthTokens tokens;

            try {
                tokens = await UseRefreshTokenAsync(
                    oauthTokens.Value.RefreshToken,
                    _options.ClientId,
                    _options.ClientSecret!,
                    _options.GetTokenEndpoint(),
                    _backchannelHttpClient,
                    _timeProvider
                );
            }
            catch (HttpRequestException e) when (e.StatusCode == System.Net.HttpStatusCode.BadRequest) {
                LogRefreshBadRequestError(e);
                return null;
            }

            await SaveTokensAsync(tokens);

            return tokens;
        }


        /// <inheritdoc/>
        async ValueTask ITokenStore.SaveTokensAsync(OAuthTokens tokens) {
            if (!Ready) {
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
        /// <param name="clientId">
        ///   The OAuth client ID for the application.
        /// </param>
        /// <param name="clientSecret">
        ///   The OAuth client secret for the application. Can be <see langword="null"/>.
        /// </param>
        /// <param name="tokenEndpoint">
        ///   The OAuth token endpoint to use.
        /// </param>
        /// <param name="httpClient">
        ///   The HTTP client to use.
        /// </param>
        /// <param name="timeProvider">
        ///   The time provider to use when determining the expiry time for the new access token.
        /// </param>
        /// <returns>
        ///   The OAuth token response.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="refreshToken"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="clientId"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="tokenEndpoint"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="httpClient"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="timeProvider"/> is <see langword="null"/>.
        /// </exception>
        public static async Task<OAuthTokens> UseRefreshTokenAsync(
            string refreshToken,
            string clientId,
            string clientSecret,
            string tokenEndpoint,
            HttpClient httpClient,
            TimeProvider timeProvider
        ) {
            if (refreshToken == null) {
                throw new ArgumentNullException(nameof(refreshToken));
            }
            if (clientId == null) {
                throw new ArgumentNullException(nameof(clientId));
            }
            if (tokenEndpoint == null) {
                throw new ArgumentNullException(nameof(tokenEndpoint));
            }
            if (httpClient == null) {
                throw new ArgumentNullException(nameof(httpClient));
            }
            if (timeProvider == null) {
                throw new ArgumentNullException(nameof(timeProvider));
            }

            var tokenRequestParameters = new Dictionary<string, string>() {
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = refreshToken
            };

            tokenRequestParameters["client_id"] = clientId;
            if (!string.IsNullOrWhiteSpace(clientSecret) && !string.Equals(clientSecret, IndustrialAppStoreAuthenticationExtensions.DefaultClientSecret, StringComparison.OrdinalIgnoreCase)) {
                tokenRequestParameters["client_secret"] = clientSecret;
            }

            var refreshRequestContent = new FormUrlEncodedContent(tokenRequestParameters!);
            var refreshRequest = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint) {
                Content = refreshRequestContent
            };
            refreshRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var refreshResponse = await httpClient.SendAsync(refreshRequest, default);
            refreshResponse.EnsureSuccessStatusCode();

            var tokenResponseJson = System.Text.Json.JsonDocument.Parse(await refreshResponse.Content.ReadAsStreamAsync());
            var tokensResponse = OAuthTokenResponse.Success(tokenResponseJson);

            TimeSpan? expiresIn = null;
            if (int.TryParse(tokensResponse.ExpiresIn, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)) {
                expiresIn = TimeSpan.FromSeconds(value);
            }

            var tokens = CreateOAuthTokens(tokensResponse.TokenType!, tokensResponse.AccessToken!, expiresIn, tokensResponse.RefreshToken, timeProvider);
            return tokens;
        }


        /// <summary>
        /// Creates a new <see cref="OAuthTokens"/> instance.
        /// </summary>
        /// <param name="tokenType">
        ///   The access token type.
        /// </param>
        /// <param name="accessToken">
        ///   The access token.
        /// </param>
        /// <param name="expiresIn">
        ///   The token validity period.
        /// </param>
        /// <param name="refreshToken">
        ///   The refresh token.
        /// </param>
        /// <param name="timeProvider">
        ///   The system clock.
        /// </param>
        /// <returns>
        ///   A new <see cref="OAuthTokens"/> instance.
        /// </returns>
        internal static OAuthTokens CreateOAuthTokens(string tokenType, string accessToken, TimeSpan? expiresIn, string? refreshToken, TimeProvider timeProvider) {
            DateTimeOffset? utcExpiresAt = null;

            if (expiresIn != null) {
                utcExpiresAt = timeProvider.GetUtcNow().Add(expiresIn.Value);
            }

            return new OAuthTokens(tokenType, accessToken, refreshToken, utcExpiresAt);
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


        [LoggerMessage(1, LogLevel.Warning, "HTTP 400/Bad Request response while using refresh token. This indicates that the refresh token has expired or is invalid.")]
        partial void LogRefreshBadRequestError(Exception error);

    }
}
