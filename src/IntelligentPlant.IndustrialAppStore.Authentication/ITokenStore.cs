using System.Threading.Tasks;

namespace IntelligentPlant.IndustrialAppStore.Authentication {

    /// <summary>
    /// <see cref="ITokenStore"/> is used to retrieve the Industrial App Store access token for an 
    /// authenticated user.
    /// </summary>
    /// <remarks>
    ///   <see cref="ITokenStore"/> is registered as a scoped service. All <see cref="ITokenStore"/> 
    ///   implementations must inherit from <see cref="TokenStore"/>.
    /// </remarks>
    /// <seealso cref="TokenStore"/>
    public interface ITokenStore {

        /// <summary>
        /// Gets the access token for the authenticated user.
        /// </summary>
        /// <returns>
        ///   A <see cref="ValueTask{TResult}"/> that will return either the tokens for the 
        ///   authenticated user, or <see langword="null"/> if the access token is unavailable or 
        ///   has expired.
        /// </returns>
        ValueTask<OAuthTokens?> GetTokensAsync();

        /// <summary>
        /// Saves tokens for the authenticated user.
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
