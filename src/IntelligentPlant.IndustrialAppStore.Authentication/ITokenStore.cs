using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;

namespace IntelligentPlant.IndustrialAppStore.Authentication {

    /// <summary>
    /// <see cref="ITokenStore"/> is used to retrieve the Industrial App Store access token for an 
    /// authenticated user.
    /// </summary>
    /// <remarks>
    ///   <see cref="ITokenStore"/> is registered as a scoped service. All <see cref="ITokenStore"/> 
    ///   implementations should inherit from <see cref="TokenStore"/>.
    /// </remarks>
    /// <seealso cref="TokenStore"/>
    public interface ITokenStore {

        /// <summary>
        /// Initialises the <see cref="ITokenStore"/> using the specified settings.
        /// </summary>
        /// <param name="userId">
        ///   The user ID of the authenticated user.
        /// </param>
        /// <param name="sessionId">
        ///   The login session ID for the authenticated user.
        /// </param>
        /// <param name="properties">
        ///   The <see cref="AuthenticationProperties"/> for the authenticated user.
        /// </param>
        /// <returns>
        ///   A <see cref="ValueTask"/> that will perform any required implementation-specific 
        ///   initialisation.
        /// </returns>
        ValueTask InitAsync(string userId, string sessionId, AuthenticationProperties properties);

        /// <summary>
        /// Gets the tokens associated with the authenticated user.
        /// </summary>
        /// <returns>
        ///   A <see cref="ValueTask{TResult}"/> that will return either the tokens for the 
        ///   authenticated user, or <see langword="null"/> if the access token is unavailable or 
        ///   has expired.
        /// </returns>
        ValueTask<OAuthTokens?> GetTokensAsync();

        /// <summary>
        /// Saves tokens associated with the authenticated user.
        /// </summary>
        /// <param name="tokens">
        ///   The tokens to save.
        /// </param>
        /// <returns>
        ///   A <see cref="ValueTask"/> that will save the tokens.
        /// </returns>
        ValueTask SaveTokensAsync(OAuthTokens tokens);

    }
}
