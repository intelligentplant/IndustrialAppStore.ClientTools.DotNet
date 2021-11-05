using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace IntelligentPlant.IndustrialAppStore.Authentication {

    /// <summary>
    /// Default <see cref="ITokenStore"/> implementation that retrieves tokens from the 
    /// authentication session.
    /// </summary>
    internal class DefaultTokenStore : TokenStore {

        private AuthenticationProperties _authProperties;


        /// <summary>
        /// Creates a new <see cref="DefaultTokenStore"/> object.
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
        public DefaultTokenStore(
            IndustrialAppStoreAuthenticationOptions options, 
            HttpClient httpClient,
            ISystemClock clock
        ) : base(options, httpClient, clock) { }


        protected internal override ValueTask InitAsync(string userId, string sessionId, AuthenticationProperties properties) {
            _authProperties = properties;
            return new ValueTask();
        }


        protected override Task<OAuthTokens?> GetTokensAsync() {
            if (_authProperties == null) {
                throw new InvalidOperationException(Resources.Error_AuthenticationSessionIsRequired);
            }

            var accessToken = _authProperties.GetTokenValue(IndustrialAppStoreAuthenticationDefaults.AccessTokenName);
            if (string.IsNullOrWhiteSpace(accessToken)) {
                return null;
            }

            var tokenType = _authProperties.GetTokenValue(IndustrialAppStoreAuthenticationDefaults.TokenTypeTokenName);

            var expiresAt = _authProperties.GetTokenValue(IndustrialAppStoreAuthenticationDefaults.ExpiresAtTokenName);
            DateTimeOffset? accessTokenExpiry = null;

            if (!string.IsNullOrWhiteSpace(expiresAt) && DateTimeOffset.TryParseExact(expiresAt, "o", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var exp)) {
                accessTokenExpiry = exp;
            }

            var refreshToken = _authProperties.GetTokenValue(IndustrialAppStoreAuthenticationDefaults.RefreshTokenName);

            return Task.FromResult((OAuthTokens?) new OAuthTokens(tokenType, accessToken, refreshToken, accessTokenExpiry));
        }


        protected internal override Task SaveTokensAsync(OAuthTokens tokens) {
            if (_authProperties == null) {
                throw new InvalidOperationException(Resources.Error_AuthenticationSessionIsRequired);
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

            if (tokens.UtcExpiresAt != null) {
                // https://www.w3.org/TR/xmlschema-2/#dateTime
                // https://msdn.microsoft.com/en-us/library/az4se3k1(v=vs.110).aspx
                authTokens.Add(new AuthenticationToken {
                    Name = IndustrialAppStoreAuthenticationDefaults.ExpiresAtTokenName,
                    Value = tokens.UtcExpiresAt.Value.ToString("o", CultureInfo.InvariantCulture)
                });
            }

            _authProperties.StoreTokens(authTokens);

            return Task.CompletedTask;
        }

    }
}
