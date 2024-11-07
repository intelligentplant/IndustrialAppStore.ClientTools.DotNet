using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

using IntelligentPlant.IndustrialAppStore.CommandLine.Http;
using IntelligentPlant.IndustrialAppStore.CommandLine.OAuth;

using Microsoft.AspNetCore.DataProtection;

namespace IntelligentPlant.IndustrialAppStore.CommandLine {

    /// <summary>
    /// <see cref="IndustrialAppStoreSessionManager"/> manages access tokens and refresh tokens 
    /// obtained from the Industrial App Store.
    /// </summary>
    public sealed class IndustrialAppStoreSessionManager {

        /// <summary>
        /// The HTTP client factory to create backchannel clients with.
        /// </summary>
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// The data protector for protecting and unprotecting tokens at rest.
        /// </summary>
        private readonly IDataProtector _dataProtector;

        /// <summary>
        /// The options for the session manager.
        /// </summary>
        private readonly IndustrialAppStoreSessionManagerOptions _options;

        /// <summary>
        /// The time provider.
        /// </summary>
        private readonly TimeProvider _timeProvider;

        /// <summary>
        /// The file to store tokens in.
        /// </summary>
        private readonly FileInfo? _tokenFile;

        /// <summary>
        /// Lock for synchronizing access to the tokens.
        /// </summary>
        private readonly Nito.AsyncEx.AsyncLock _tokensLock = new Nito.AsyncEx.AsyncLock();

        /// <summary>
        /// The current tokens.
        /// </summary>
        private OAuthTokens? _tokens;


        /// <summary>
        /// Creates a new <see cref="IndustrialAppStoreSessionManager"/> instance.
        /// </summary>
        /// <param name="httpClientFactory">
        ///   The HTTP client factory to create backchannel clients with.
        /// </param>
        /// <param name="dataProtectionProvider">
        ///   The data protection provider for protecting and unprotecting tokens at rest.
        /// </param>
        /// <param name="options">
        ///   The options for the session manager.
        /// </param>
        /// <param name="timeProvider">
        ///   The time provider. If <see langword="null"/>, <see cref="TimeProvider.System"/> will be used.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="httpClientFactory"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="dataProtectionProvider"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="options"/> is <see langword="null"/>.
        /// </exception>
        public IndustrialAppStoreSessionManager(
            IHttpClientFactory httpClientFactory,
            IDataProtectionProvider dataProtectionProvider,
            IndustrialAppStoreSessionManagerOptions options,
            TimeProvider? timeProvider = null
        ) {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            if (dataProtectionProvider == null) {
                throw new ArgumentNullException(nameof(dataProtectionProvider));
            }
            _dataProtector = dataProtectionProvider.CreateProtector(typeof(IndustrialAppStoreSessionManager).FullName!, "tokens", "v1");
            _options = options ?? throw new ArgumentNullException(nameof(options));

            if (_options.TokenPath != null) {
                var tokenFileName = BitConverter.ToString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(_options.IndustrialAppStoreUrl.Authority))).Replace("-", "");
                var tokenFileFullPath = Path.IsPathRooted(_options.TokenPath)
                    ? Path.Combine(_options.TokenPath, tokenFileName)
                    : Path.Combine(
                        Environment.UserInteractive
                            ? Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                            : Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                        _options.TokenPath,
                        tokenFileName);
                _tokenFile = new FileInfo(tokenFileFullPath);
            }

            _timeProvider = timeProvider ?? TimeProvider.System;
        }


        /// <summary>
        /// Gets the absolute URI for the device authorization endpoint.
        /// </summary>
        /// <returns>
        ///   The absolute URI for the device authorization endpoint.
        /// </returns>
        private Uri GetDeviceAuthorizationEndpoint() => new Uri(_options.IndustrialAppStoreUrl, Constants.DeviceAuthorizationEndpoint);


        /// <summary>
        /// Gets the absolute URI for the token endpoint.
        /// </summary>
        /// <returns>
        ///   The absolute URI for the token endpoint.
        /// </returns>
        private Uri GetTokenEndpoint() => new Uri(_options.IndustrialAppStoreUrl, Constants.TokenEndpoint);


        /// <summary>
        /// Encrypts and saves the tokens to disk.
        /// </summary>
        /// <param name="tokens">
        ///   The tokens to save.
        /// </param>
        private void SaveTokens(OAuthTokens tokens) {
            _tokens = tokens;

            if (_tokenFile == null) {
                return;
            }

            var tokenData = JsonSerializer.SerializeToUtf8Bytes(tokens);
            var protectedTokenData = _dataProtector.Protect(tokenData);

            _tokenFile.Directory!.Create();
            File.WriteAllBytes(_tokenFile.FullName, protectedTokenData);
        }


        /// <summary>
        /// Loads the tokens from disk.
        /// </summary>
        /// <returns>
        ///   The loaded tokens, if available.
        /// </returns>
        private OAuthTokens? LoadTokens() {
            if (_tokenFile == null) {
                return null;
            }

            _tokenFile.Refresh();
            if (!_tokenFile.Exists) {
                return null;
            }

            var protectedTokenData = File.ReadAllBytes(_tokenFile.FullName);
            var tokenData = _dataProtector.Unprotect(protectedTokenData);
            return JsonSerializer.Deserialize<OAuthTokens>(tokenData);
        }


        /// <summary>
        /// Gets the access token for the current authentication session.
        /// </summary>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The access token, or <see langword="null"/> if no active authentication session 
        ///   exists or the access token has expired and cannot be refreshed.
        /// </returns>
        public async ValueTask<string?> GetAccessTokenAsync(CancellationToken cancellationToken) {
            using var handle = await _tokensLock.LockAsync(cancellationToken).ConfigureAwait(false);
            return await GetAccessTokenCoreAsync(cancellationToken).ConfigureAwait(false);
        }


        /// <summary>
        /// Gets the access token for the current authentication session.
        /// </summary>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The access token, or <see langword="null"/> if no active authentication session 
        ///   exists or the access token has expired and cannot be refreshed.
        /// </returns>
        private async ValueTask<string?> GetAccessTokenCoreAsync(CancellationToken cancellationToken) {
            var tokens = _tokens ??= LoadTokens();
            if (tokens == null) {
                return null;
            }

            if (!tokens.UtcExpiresAt.HasValue || tokens.UtcExpiresAt.Value > _timeProvider.GetUtcNow()) {
                return tokens.AccessToken;
            }

            if (string.IsNullOrWhiteSpace(tokens.RefreshToken)) {
                return null;
            }

            tokens = await UseRefreshTokenAsync(tokens.RefreshToken!, cancellationToken).ConfigureAwait(false);
            SaveTokens(tokens);

            return tokens.AccessToken;
        }


        /// <summary>
        /// Exchanges a refresh token for a new set of tokens.
        /// </summary>
        /// <param name="refreshToken">
        ///   The refresh token.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The OAuth token response.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="refreshToken"/> is <see langword="null"/>.
        /// </exception>
        private async Task<OAuthTokens> UseRefreshTokenAsync(
            string refreshToken,
            CancellationToken cancellationToken
        ) {
            var tokenRequestParameters = new Dictionary<string, string>() {
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = refreshToken
            };

            tokenRequestParameters["client_id"] = _options.ClientId;
            if (!string.IsNullOrWhiteSpace(_options.ClientSecret)) {
                tokenRequestParameters["client_secret"] = _options.ClientSecret!;
            }

            var refreshRequestContent = new FormUrlEncodedContent(tokenRequestParameters!);
            var refreshRequest = new HttpRequestMessage(HttpMethod.Post, GetTokenEndpoint()) {
                Content = refreshRequestContent
            };
            refreshRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var httpClient = _httpClientFactory.CreateClient(nameof(IndustrialAppStoreSessionManager));
            var refreshResponse = await httpClient.SendAsync(refreshRequest, cancellationToken).ConfigureAwait(false);
            await refreshResponse.ThrowOnOAuthErrorResponseAsync(cancellationToken).ConfigureAwait(false);

            var tokensResponse = await refreshResponse.Content.ReadFromJsonAsync<OAuthTokenEndpointResponse>(cancellationToken).ConfigureAwait(false);
            return new OAuthTokens(tokensResponse?.AccessToken!, tokensResponse?.RefreshToken, _timeProvider.GetUtcNow().AddSeconds(tokensResponse?.ExpiresIn ?? 86_400));
        }


        /// <summary>
        /// Signs into the Industrial App Store if an active authentication session does not 
        /// already exist.
        /// </summary>
        /// <param name="callback">
        ///   A delegate that will be invoked if a new device authorization request is created. 
        ///   The delegate should inform the user that they must approve the device authorization 
        ///   request before sign-in can continue.
        /// </param>
        /// <param name="forceNewSession">
        ///   When <see langword="true"/>, a new device authorization request will always be 
        ///   created even if an active authentication session aleady exists.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A <see cref="ValueTask{TResult}"/> that completes when the operation has completed and returns 
        ///   <see langword="true"/> if a new authentication session was created, or <see langword="false"/> 
        ///   if an existing authentication session was available.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="callback"/> is <see langword="null"/>.
        /// </exception>
        /// <remarks>
        ///   If authentication fails or times out an exception will be thrown.
        /// </remarks>
        public async ValueTask<bool> SignInAsync(DeviceAuthorizationRequestCreatedDelegate callback, bool forceNewSession = false, CancellationToken cancellationToken = default) {
            using var handle = await _tokensLock.LockAsync(cancellationToken).ConfigureAwait(false);

            if (!forceNewSession) {
                var accessToken = await GetAccessTokenCoreAsync(cancellationToken).ConfigureAwait(false);
                if (accessToken != null) {
                    return false;
                }
            }

            var httpClient = _httpClientFactory.CreateClient(nameof(IndustrialAppStoreSessionManager));

            var tokens = await httpClient.AuthenticateWithDeviceCodeAsync(new DeviceLoginOptions() {
                ClientId = _options.ClientId,
                ClientSecret = _options.ClientSecret,
                Scopes = _options.Scopes,
                DeviceAuthorizationEndpoint = GetDeviceAuthorizationEndpoint(),
                TokenEndpoint = GetTokenEndpoint(),
                OnRequestCreated = callback
            }, _timeProvider, cancellationToken).ConfigureAwait(false);

            SaveTokens(tokens);
            return true;
        }


        /// <summary>
        /// Clears the active authentication session if one exists.
        /// </summary>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A <see cref="ValueTask"/> that completes when the operation has completed.
        /// </returns>
        public async ValueTask SignOutAsync(CancellationToken cancellationToken) {
            using var handle = await _tokensLock.LockAsync(cancellationToken).ConfigureAwait(false);

            if (_tokenFile != null) {
                _tokenFile.Refresh();
                if (_tokenFile.Exists) {
                    _tokenFile.Delete();
                }
            }

            _tokens = null;
        }

    }
}
