using System.Net.Http;

using IntelligentPlant.IndustrialAppStore.DependencyInjection;

using Microsoft.Extensions.Options;

namespace IntelligentPlant.IndustrialAppStore.Authentication.Http {

    /// <summary>
    /// <see cref="IIndustrialAppStoreHttpFactory"/> implementation that authenticates outgoing 
    /// requests using the registered <see cref="ITokenStore"/> for the current scope.
    /// </summary>
    internal sealed class TokenStoreHttpFactory : IndustrialAppStoreHttpFactory {

        /// <summary>
        /// The token store.
        /// </summary>
        private readonly ITokenStore _tokenStore;

        /// <summary>
        /// The authentication options.
        /// </summary>
        private readonly IOptionsSnapshot<IndustrialAppStoreAuthenticationOptions> _options;


        /// <summary>
        /// Creates a new <see cref="TokenStoreHttpFactory"/> instance.
        /// </summary>
        /// <param name="httpMessageHandlerFactory">
        ///   The HTTP message handler factory.
        /// </param>
        /// <param name="tokenStore">
        ///   The token store.
        /// </param>
        /// <param name="options">
        ///   The authentication options.
        /// </param>
        public TokenStoreHttpFactory(IHttpMessageHandlerFactory httpMessageHandlerFactory, ITokenStore tokenStore, IOptionsSnapshot<IndustrialAppStoreAuthenticationOptions> options) : base(httpMessageHandlerFactory) {
            _tokenStore = tokenStore;
            _options = options;
        }


        /// <inheritdoc/>
        protected override HttpMessageHandler CreateHandler() {
            var httpHandler = base.CreateHandler();

            if (!_options.Value.UseExternalAuthentication) {
                // We're using the token store for authentication, so add an authentication
                // handler to the start of the pipeline.
                httpHandler = Jaahas.Http.HttpClientFactory.CreatePipeline(httpHandler, new TokenStoreAuthenticationHandler(_tokenStore));
            }

            return httpHandler;
        }

    }
}
