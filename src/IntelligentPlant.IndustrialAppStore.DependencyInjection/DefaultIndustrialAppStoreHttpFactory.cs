using IntelligentPlant.IndustrialAppStore.DependencyInjection.Internal;

namespace IntelligentPlant.IndustrialAppStore.DependencyInjection {

    /// <summary>
    /// Default implementation of <see cref="IIndustrialAppStoreHttpFactory"/>.
    /// </summary>
    public sealed class DefaultIndustrialAppStoreHttpFactory : IndustrialAppStoreHttpFactory {

        /// <summary>
        /// The access token provider.
        /// </summary>
        private readonly AccessTokenProvider _accessTokenProvider;


        /// <summary>
        /// Creates a new <see cref="DefaultIndustrialAppStoreHttpFactory"/> instance.
        /// </summary>
        /// <param name="httpMessageHandlerFactory">
        ///   The HTTP message handler factory.
        /// </param>
        /// <param name="accessTokenProvider">
        ///   The access token provider to use.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        ///   <paramref name="httpMessageHandlerFactory"/> is <see langword="null"/>.
        /// </exception>
        public DefaultIndustrialAppStoreHttpFactory(IHttpMessageHandlerFactory httpMessageHandlerFactory, AccessTokenProvider accessTokenProvider)
            : base(httpMessageHandlerFactory) {
            _accessTokenProvider = accessTokenProvider ?? throw new ArgumentNullException(nameof(accessTokenProvider));
        }


        /// <inheritdoc/>
        protected override HttpMessageHandler CreateHandler() {
            var httpHandler = base.CreateHandler();
            return Jaahas.Http.HttpClientFactory.CreatePipeline(
                httpHandler, 
                new AccessTokenAuthenticationHandler(_accessTokenProvider));
        }

    }

}
