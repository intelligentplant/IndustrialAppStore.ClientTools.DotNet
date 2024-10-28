using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace IntelligentPlant.IndustrialAppStore.Authentication {

    /// <summary>
    /// Default <see cref="ITokenStore"/> implementation that retrieves tokens from the 
    /// authentication session.
    /// </summary>
    internal sealed class AuthenticationPropertiesTokenStore : TokenStore {

        /// <summary>
        /// The authentication session to store tokens in.
        /// </summary>
        private AuthenticationProperties? _authenticationProperties;


        /// <summary>
        /// Creates a new <see cref="AuthenticationPropertiesTokenStore"/> object.
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
        public AuthenticationPropertiesTokenStore(
            IOptions<IndustrialAppStoreAuthenticationOptions> options, 
            HttpClient httpClient,
            TimeProvider timeProvider
        ) : base(options, httpClient, timeProvider) { }


        /// <summary>
        /// Initialises the <see cref="AuthenticationPropertiesTokenStore"/>.
        /// </summary>
        /// <param name="userId">
        ///   The user ID.
        /// </param>
        /// <param name="sessionId">
        ///   The session ID.
        /// </param>
        /// <param name="authenticationProperties">
        ///   The authentication properties for the session.
        /// </param>
        /// <returns>
        ///   A <see cref="ValueTask"/> that will initialise the <see cref="AuthenticationPropertiesTokenStore"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="userId"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="sessionId"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="authenticationProperties"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///   The token store has already been initialised.
        /// </exception>
        public async ValueTask InitAsync(string userId, string sessionId, AuthenticationProperties authenticationProperties) {
            if (authenticationProperties == null) {
                throw new ArgumentNullException(nameof(authenticationProperties));
            }
            await InitCoreAsync(userId, sessionId).ConfigureAwait(false);
            _authenticationProperties = authenticationProperties;
        }


        /// <inheritdoc/>
        protected override ValueTask InitAsync() {
            return default;
        }


        /// <inheritdoc/>
        protected override ValueTask<OAuthTokens?> GetTokensAsync() {
            if (_authenticationProperties == null) {
                throw new InvalidOperationException(Resources.Error_TokenStoreHasNotBeenInitialised);
            }

            var accessToken = _authenticationProperties.GetTokenValue(IndustrialAppStoreAuthenticationDefaults.AccessTokenName);
            if (string.IsNullOrWhiteSpace(accessToken)) {
                return new ValueTask<OAuthTokens?>();
            }

            var tokenType = _authenticationProperties.GetTokenValue(IndustrialAppStoreAuthenticationDefaults.TokenTypeTokenName);

            var expiresAt = _authenticationProperties.GetTokenValue(IndustrialAppStoreAuthenticationDefaults.ExpiresAtTokenName);
            DateTimeOffset? accessTokenExpiry = null;

            if (!string.IsNullOrWhiteSpace(expiresAt) && DateTimeOffset.TryParseExact(expiresAt, "o", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var exp)) {
                accessTokenExpiry = exp;
            }

            var refreshToken = _authenticationProperties.GetTokenValue(IndustrialAppStoreAuthenticationDefaults.RefreshTokenName);

            return new ValueTask<OAuthTokens?>(new OAuthTokens(tokenType!, accessToken, refreshToken!, accessTokenExpiry));
        }


        /// <inheritdoc/>
        protected internal override ValueTask SaveTokensAsync(OAuthTokens tokens) {
            if (_authenticationProperties == null) {
                throw new InvalidOperationException(Resources.Error_TokenStoreHasNotBeenInitialised);
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

            _authenticationProperties.StoreTokens(authTokens);

            return default;
        }

    }
}
