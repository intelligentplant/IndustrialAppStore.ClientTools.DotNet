using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;

namespace IntelligentPlant.IndustrialAppStore.Authentication {

    /// <summary>
    /// Extensions for <see cref="ITokenStore"/>.
    /// </summary>
    public static class TokenStoreExtensions {

        /// <summary>
        /// Initialises the token store using the specified principal and authentication properties.
        /// </summary>
        /// <param name="tokenStore">
        ///   The token store.
        /// </param>
        /// <param name="principal">
        ///   The principal to retrieve the user ID and session ID from.
        /// </param>
        /// <param name="properties">
        ///   The authentication properties.
        /// </param>
        /// <returns>
        ///   A <see cref="ValueTask"/> that will initialise the token store.
        /// </returns>
        internal static async ValueTask InitTokenStoreAsync(this ITokenStore tokenStore, ClaimsPrincipal principal, AuthenticationProperties properties) {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var sessionId = principal.FindFirstValue(IndustrialAppStoreAuthenticationDefaults.AppSessionIdClaimType);
            if (tokenStore is AuthenticationPropertiesTokenStore defaultStore) {
                await defaultStore.InitAsync(userId!, sessionId!, properties).ConfigureAwait(false);
            }
            else {
                await tokenStore.InitAsync(userId!, sessionId!).ConfigureAwait(false);
            }
        }


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
        public static async Task<AuthenticationHeaderValue?> GetAuthenticationHeaderAsync(this ITokenStore tokenStore) {
            if (tokenStore == null) {
                throw new ArgumentNullException(nameof(tokenStore));
            }

            var accessToken = await tokenStore.GetTokensAsync();
            if (string.IsNullOrWhiteSpace(accessToken?.AccessToken)) {
                return null;
            }

            return new AuthenticationHeaderValue("Bearer", accessToken.Value.AccessToken);
        }

    }
}
