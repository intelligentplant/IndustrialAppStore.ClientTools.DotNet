namespace IntelligentPlant.IndustrialAppStore.DependencyInjection {

    /// <summary>
    /// <see cref="AccessTokenProvider"/> is used by the default <see cref="IIndustrialAppStoreHttpFactory"/> 
    /// implementation (<see cref="DefaultIndustrialAppStoreHttpFactory"/>) to provide an access 
    /// token for outgoing requests.
    /// </summary>
    public class AccessTokenProvider {

        /// <summary>
        /// The delegate that will retrieve the access token.
        /// </summary>
        public AccessTokenFactory? Factory { get; set; }


        /// <summary>
        /// Creates a new <see cref="AccessTokenFactory"/> that uses a static access token.
        /// </summary>
        /// <param name="accessToken">
        ///   The access token.
        /// </param>
        /// <returns>
        ///   An <see cref="AccessTokenFactory"/> delegate that always returns the provided 
        ///   <paramref name="accessToken"/>.
        /// </returns>
        public static AccessTokenFactory CreateStaticAccessTokenFactory(string? accessToken) {
            return _ => new ValueTask<string?>(accessToken);
        }

    }


    /// <summary>
    /// Delegate that is used to provide an access token for outgoing requests.
    /// </summary>
    /// <param name="cancellationToken">
    ///   The cancellation token for the operation.
    /// </param>
    /// <returns>
    ///   A <see cref="ValueTask{TResult}"/> that will return the access token to use, if available.
    /// </returns>
    public delegate ValueTask<string?> AccessTokenFactory(CancellationToken cancellationToken);

}
