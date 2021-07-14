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
    /// Extenrions for <see cref="AuthenticationProperties"/>.
    /// </summary>
    public static class AuthenticationPropertiesExtensions {

        /// <inheritdoc/>
        public static async Task<string> GetAccessTokenAsync(
            this AuthenticationProperties properties,
            IndustrialAppStoreAuthenticationOptions options,
            HttpClient backchannel,
            ISystemClock clock,
            CancellationToken cancellationToken = default
        ) {
            var accessToken = properties.GetTokenValue(IndustrialAppStoreAuthenticationDefaults.AccessTokenName);

            if (string.IsNullOrWhiteSpace(accessToken)) {
                return null;
            }

            var expiresAt = properties.GetTokenValue(IndustrialAppStoreAuthenticationDefaults.ExpiresAtTokenName);

            if (string.IsNullOrWhiteSpace(expiresAt) || !DateTime.TryParseExact(expiresAt, "o", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var accessTokenExpiry)) {
                // No/invalid expiry specified (this would be unusual).
                return accessToken;
            }

            if (accessTokenExpiry > DateTime.UtcNow) {
                // Access token is still valid.
                return accessToken;
            }

            // Access token has expired. If we have a refresh token, we will try and use it.
            var refreshToken = properties.GetTokenValue(IndustrialAppStoreAuthenticationDefaults.RefreshTokenName);

            if (string.IsNullOrWhiteSpace(refreshToken)) {
                // No refresh token available.
                return null;
            }

            var tokenRequestParameters = new Dictionary<string, string>() {
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = refreshToken
            };

#if NETCOREAPP
            tokenRequestParameters["client_id"] = options.ClientId;
            if (!string.IsNullOrWhiteSpace(options.ClientSecret) && !string.Equals(options.ClientSecret, IndustrialAppStoreAuthenticationExtensions.DefaultClientSecret, StringComparison.OrdinalIgnoreCase)) {
                tokenRequestParameters["client_secret"] = options.ClientSecret;
            }
#else
            tokenRequestParameters["client_id"] = options.ClientId;
            tokenRequestParameters["client_secret"] = options.ClientSecret;
#endif

            var refreshRequestContent = new FormUrlEncodedContent(tokenRequestParameters);
            var refreshRequest = new HttpRequestMessage(HttpMethod.Post, options.GetTokenEndpoint()) {
                Content = refreshRequestContent
            };
            refreshRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var refreshResponse = await backchannel.SendAsync(refreshRequest, cancellationToken);

#if NETCOREAPP
            var tokenResponseJson = System.Text.Json.JsonDocument.Parse(await refreshRequest.Content.ReadAsStreamAsync());
#else
            var tokenResponseJson = Newtonsoft.Json.Linq.JObject.Parse(await refreshResponse.Content.ReadAsStringAsync());
#endif
            var tokens = OAuthTokenResponse.Success(tokenResponseJson);
            SaveTokens(tokens, properties, clock);

            return tokens.AccessToken;
        }


        /// <summary>
        /// Saves the specified OAuth tokens to the authentication session.
        /// </summary>
        /// <param name="tokens">
        ///   The OAuth tokens.
        /// </param>
        /// <param name="properties">
        ///   The authentication session properties.
        /// </param>
        /// <param name="clock">
        ///   The system clock.
        /// </param>
        private static void SaveTokens(OAuthTokenResponse tokens, AuthenticationProperties properties, ISystemClock clock) {
            if (tokens == null) {
                throw new ArgumentNullException(nameof(tokens));
            }
            if (properties == null) {
                throw new ArgumentNullException(nameof(properties));
            }

            var authTokens = new List<AuthenticationToken>();

            authTokens.Add(new AuthenticationToken {
                Name = IndustrialAppStoreAuthenticationDefaults.AccessTokenName,
                Value = tokens.AccessToken
            });

            if (!string.IsNullOrEmpty(tokens.RefreshToken)) {
                authTokens.Add(new AuthenticationToken {
                    Name = IndustrialAppStoreAuthenticationDefaults.RefreshTokenName,
                    Value = tokens.RefreshToken
                });
            }

            if (!string.IsNullOrEmpty(tokens.TokenType)) {
                authTokens.Add(new AuthenticationToken {
                    Name = IndustrialAppStoreAuthenticationDefaults.TokenTypeTokenName,
                    Value = tokens.TokenType
                });
            }

            if (!string.IsNullOrEmpty(tokens.ExpiresIn)) {
                int value;
                if (int.TryParse(tokens.ExpiresIn, NumberStyles.Integer, CultureInfo.InvariantCulture, out value)) {
                    // https://www.w3.org/TR/xmlschema-2/#dateTime
                    // https://msdn.microsoft.com/en-us/library/az4se3k1(v=vs.110).aspx
                    var expiry = clock.UtcNow + TimeSpan.FromSeconds(value);
                    authTokens.Add(new AuthenticationToken {
                        Name = IndustrialAppStoreAuthenticationDefaults.ExpiresAtTokenName,
                        Value = expiry.ToString("o", CultureInfo.InvariantCulture)
                    });
                }
            }

            properties.StoreTokens(authTokens);
        }

    }
}
