using IntelligentPlant.IndustrialAppStore.DependencyInjection.Internal;

namespace IntelligentPlant.IndustrialAppStore.DependencyInjection {

    /// <summary>
    /// Default implementation of <see cref="IIndustrialAppStoreHttpFactory"/>.
    /// </summary>
    /// <remarks>
    /// 
    /// <para>
    ///   <see cref="DefaultIndustrialAppStoreHttpFactory"/> uses the <see cref="AccessTokenProvider"/> 
    ///   service to retrieve access tokens to use when authenticating outgoing requests.
    /// </para>
    /// 
    /// <para>
    ///   The primary <see cref="HttpMessageHandler"/> for <see cref="HttpClient"/> instances 
    ///   created by a <see cref="DefaultIndustrialAppStoreHttpFactory"/> is created using an 
    ///   <see cref="IHttpMessageHandlerFactory"/>. This allows the same primary handler to be 
    ///   re-used across multiple <see cref="HttpClient"/> instances while allowing each instance 
    ///   to define its own handler for applying authentication headers.
    /// </para>
    /// 
    /// </remarks>
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
        /// <exception cref="ArgumentNullException">
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
