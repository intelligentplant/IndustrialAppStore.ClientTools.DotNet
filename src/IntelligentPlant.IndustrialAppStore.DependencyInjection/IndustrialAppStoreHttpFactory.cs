using IntelligentPlant.IndustrialAppStore.Client;

namespace IntelligentPlant.IndustrialAppStore.DependencyInjection {

    /// <summary>
    /// Base <see cref="IIndustrialAppStoreHttpFactory"/> implementation that delegates creation of 
    /// <see cref="HttpMessageHandler"/> instances to an <see cref="IHttpMessageHandlerFactory"/>.
    /// </summary>
    public abstract class IndustrialAppStoreHttpFactory : IIndustrialAppStoreHttpFactory {

        /// <summary>
        /// The name of the <see cref="IHttpMessageHandlerFactory"/> configuration to use when 
        /// creating <see cref="HttpMessageHandler"/>s
        /// </summary>
        internal const string HttpClientName = nameof(IndustrialAppStoreHttpClient);


        /// <summary>
        /// The HTTP message handler factory.
        /// </summary>
        private readonly IHttpMessageHandlerFactory _httpMessageHandlerFactory;


        /// <summary>
        /// Creates a new <see cref="IndustrialAppStoreHttpFactory"/> instance.
        /// </summary>
        /// <param name="httpMessageHandlerFactory">
        ///   The HTTP message handler factory.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="httpMessageHandlerFactory"/> is <see langword="null"/>.
        /// </exception>
        protected IndustrialAppStoreHttpFactory(IHttpMessageHandlerFactory httpMessageHandlerFactory) {
            _httpMessageHandlerFactory = httpMessageHandlerFactory ?? throw new ArgumentNullException(nameof(httpMessageHandlerFactory));
        }


        /// <summary>
        /// Creates a new HTTP message handler.
        /// </summary>
        /// <returns>
        ///   The HTTP message handler.
        /// </returns>
        protected virtual HttpMessageHandler CreateHandler() => _httpMessageHandlerFactory.CreateHandler(HttpClientName);


        /// <summary>
        /// Creates a new <see cref="HttpClient"/> instance.
        /// </summary>
        /// <returns>
        ///   The <see cref="HttpClient"/> instance.
        /// </returns>
        protected virtual HttpClient CreateClient() {
            var httpClient = new HttpClient(CreateHandler(), false);
            ConfigureHttpClient(httpClient);
            return httpClient;
        }


        /// <summary>
        /// Configures the specified <see cref="HttpClient"/> instance.
        /// </summary>
        /// <param name="httpClient">
        ///   The <see cref="HttpClient"/> instance to configure.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="httpClient"/> is <see langword="null"/>.
        /// </exception>
        protected virtual void ConfigureHttpClient(HttpClient httpClient) {
            if (httpClient == null) {
                throw new ArgumentNullException(nameof(httpClient));
            }

#if NET8_0_OR_GREATER
            httpClient.DefaultRequestVersion = System.Net.HttpVersion.Version11;
            httpClient.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
#endif
        }


        /// <inheritdoc/>
        HttpClient IIndustrialAppStoreHttpFactory.CreateClient() => CreateClient();

    }
}
