using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;

using IntelligentPlant.IndustrialAppStore.CommandLine.OAuth;

namespace IntelligentPlant.IndustrialAppStore.CommandLine.Http {

    /// <summary>
    /// Industrial App Store authentication extensions for <see cref="HttpClient"/>.
    /// </summary>
    public static class HttpClientExtensions {

        /// <summary>
        /// Authenticates with the Industrial App Store using the device authorization grant.
        /// </summary>
        /// <param name="httpClient">
        ///   The <see cref="HttpClient"/>.
        /// </param>
        /// <param name="options">
        ///   The device login options.
        /// </param>
        /// <param name="timeProvider">
        ///   The time provider to use for calculating timeouts. If <see langword="null"/>, 
        ///   <see cref="TimeProvider.System"/> will be used.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The OAuth tokens received from the Industrial App Store token endpoint.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="httpClient"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="options"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ValidationException">
        ///   <paramref name="options"/> fails validation.
        /// </exception>
        /// <exception cref="OAuthException">
        ///   A fatal OAuth error occurs during the authentication process.
        /// </exception>
        public static async Task<OAuthTokens> AuthenticateWithDeviceCodeAsync(
            this HttpClient httpClient,
            DeviceLoginOptions options,
            TimeProvider? timeProvider = null,
            CancellationToken cancellationToken = default
        ) {
            if (httpClient == null) {
                throw new ArgumentNullException(nameof(httpClient));
            }

            if (options == null) {
                throw new ArgumentNullException(nameof(options));
            }
            Validator.ValidateObject(options, new ValidationContext(options), true);

            var authorizeRequestBody = new Dictionary<string, string> {
                ["client_id"] = options.ClientId
            };

            if (!string.IsNullOrEmpty(options.ClientSecret)) {
                authorizeRequestBody["client_secret"] = options.ClientSecret!;
            }

            if (options.Scopes != null && options.Scopes.Any()) {
                authorizeRequestBody["scope"] = string.Join(" ", options.Scopes);
            }

            var authorizeResponse = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, options.DeviceAuthorizationEndpoint) {
                Content = new FormUrlEncodedContent(authorizeRequestBody)
            }, cancellationToken).ConfigureAwait(false);

            await authorizeResponse.ThrowOnOAuthErrorResponseAsync(cancellationToken).ConfigureAwait(false);

            var deviceAuthorizationResponse = await authorizeResponse.Content.ReadFromJsonAsync<OAuthDeviceAuthorizationEndpointResponse>(cancellationToken).ConfigureAwait(false);

            using var ctSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            ctSource.CancelAfter(TimeSpan.FromSeconds(deviceAuthorizationResponse!.ExpiresIn));

            await options.OnRequestCreated.Invoke(new PendingDeviceAuthorization(
                deviceAuthorizationResponse.VerificationUri,
                deviceAuthorizationResponse.UserCode,
                (timeProvider ?? TimeProvider.System).GetUtcNow().AddSeconds(deviceAuthorizationResponse.ExpiresIn)),
                ctSource.Token).ConfigureAwait(false);

            var tokenRequestBody = new Dictionary<string, string> {
                ["client_id"] = options.ClientId,
                ["device_code"] = deviceAuthorizationResponse.DeviceCode,
                ["grant_type"] = Constants.DeviceCodeGrantType
            };

            if (!string.IsNullOrEmpty(options.ClientSecret)) {
                tokenRequestBody["client_secret"] = options.ClientSecret!;
            }

            var interval = deviceAuthorizationResponse.Interval ?? 5;

            do {
                await Task.Delay(TimeSpan.FromSeconds(interval), ctSource.Token).ConfigureAwait(false);

                var tokenResponse = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, options.TokenEndpoint) {
                    Content = new FormUrlEncodedContent(tokenRequestBody)
                }, ctSource.Token).ConfigureAwait(false);

                if (tokenResponse.IsSuccessStatusCode) {
                    var tokenEndpointResponse = await tokenResponse.Content.ReadFromJsonAsync<OAuthTokenEndpointResponse>(ctSource.Token).ConfigureAwait(false);
                    return new OAuthTokens(tokenEndpointResponse!.AccessToken, tokenEndpointResponse!.RefreshToken, (timeProvider ?? TimeProvider.System).GetUtcNow().AddSeconds(tokenEndpointResponse.ExpiresIn ?? 86400));
                }

                if (tokenResponse.StatusCode == System.Net.HttpStatusCode.BadRequest) {
                    var errorResponse = await tokenResponse.Content.ReadFromJsonAsync<OAuthErrorResponse>(ctSource.Token).ConfigureAwait(false);
                    switch (errorResponse?.Error) {
                        case "authorization_pending":
                            continue;
                        case "slow_down":
                            interval += 5;
                            continue;
                        default:
                            throw new OAuthException(errorResponse!);

                    }
                }

                await tokenResponse.ThrowOnOAuthErrorResponseAsync(ctSource.Token).ConfigureAwait(false);
            } while (!ctSource.IsCancellationRequested);

            throw new OAuthException("Login failed.");
        }


        /// <summary>
        /// Throws an <see cref="OAuthException"/> if the response is an OAuth error response, or 
        /// an <see cref="HttpRequestException"/> for any other response that does not indicate 
        /// success.
        /// </summary>
        /// <param name="response">
        ///   The response.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A <see cref="ValueTask"/> that completes when the operation has completed.
        /// </returns>
        /// <exception cref="OAuthException">
        ///   The <paramref name="response"/> has a non-good status and contains an OAuth error response.
        /// </exception>
        /// <exception cref="HttpRequestException">
        ///   The <paramref name="response"/> has a non-good status and does not contain an OAuth error response.
        /// </exception>
        internal static async ValueTask ThrowOnOAuthErrorResponseAsync(this HttpResponseMessage response, CancellationToken cancellationToken) {
            if (response.IsSuccessStatusCode) {
                return;
            }

            if ("application/json".Equals(response.Content.Headers.ContentType?.MediaType, StringComparison.OrdinalIgnoreCase)) {
                // JSON response - assume that it is an OAuth error response
                var errorResponse = await response.Content.ReadFromJsonAsync<OAuthErrorResponse>(cancellationToken).ConfigureAwait(false);
                if (errorResponse != null) {
                    throw new OAuthException(errorResponse);
                }
            }

            // Fall back to throwing an HTTP client exception.
            response.EnsureSuccessStatusCode();
        }

    }
}
