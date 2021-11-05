using System;
using System.Globalization;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace IntelligentPlant.IndustrialAppStore.Authentication {

    /// <summary>
    /// Extensions for <see cref="ITokenStore"/>.
    /// </summary>
    public static class TokenStoreExtensions {

        /// <summary>
        /// Gets an <see cref="AuthenticationHeaderValue"/> that can be added to outgoing requests 
        /// using the access token in the token store.
        /// </summary>
        /// <param name="tokenStore">
        ///   The token store.
        /// </param>
        /// <returns>
        ///   A <see cref="Task{TResult}"/> that will return the <see cref="AuthenticationHeaderValue"/>. 
        ///   If the token store does not contain a valid access token, the return value will be 
        ///   <see langword="null"/>.
        /// </returns>
        public static async Task<AuthenticationHeaderValue> GetAuthenticationHeaderAsync(this ITokenStore tokenStore) {
            if (tokenStore == null) {
                throw new ArgumentNullException(nameof(tokenStore));
            }
            
            var accessToken = await tokenStore.GetTokensAsync();
            if (string.IsNullOrWhiteSpace(accessToken?.AccessToken)) {
                return null;
            }

            return new AuthenticationHeaderValue("Bearer", accessToken.Value.AccessToken);
        }


        internal static OAuthTokens CreateOAuthTokens(this TokenStore tokenStore, string tokenType, string accessToken, TimeSpan? expiresIn, string refreshToken) {
            DateTimeOffset? utcExpiresAt = null;

            if (expiresIn != null) {
                utcExpiresAt = tokenStore.Clock.UtcNow.Add(expiresIn.Value);
            }

            return new OAuthTokens(tokenType, accessToken, refreshToken, utcExpiresAt);
        }

    }
}
