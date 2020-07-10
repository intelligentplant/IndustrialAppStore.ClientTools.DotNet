using System.Threading.Tasks;

namespace IntelligentPlant.IndustrialAppStore.Authentication {

    /// <summary>
    /// <see cref="ITokenStore"/> is used to retrieve the Industrial App Store access token for an 
    /// authenticated user.
    /// </summary>
    /// <remarks>
    ///   Implementations of this service are registered as scoped.
    /// </remarks>
    public interface ITokenStore {

        /// <summary>
        /// Gets the access token for the authenticated user.
        /// </summary>
        /// <returns>
        ///   A <see cref="Task{TResult}"/> that will return either the access token, or 
        ///   <see langword="null"/> if the access token is unavailable or has expired.
        /// </returns>
        Task<string> GetAccessTokenAsync();

    }
}
