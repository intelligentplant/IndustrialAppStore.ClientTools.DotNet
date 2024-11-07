﻿using IntelligentPlant.IndustrialAppStore.Client;

namespace IntelligentPlant.IndustrialAppStore.DependencyInjection {

    /// <summary>
    /// Base <see cref="IIndustrialAppStoreHttpFactory"/> implementation that delegates creation of 
    /// <see cref="HttpMessageHandler"/> instances to an <see cref="IHttpMessageHandlerFactory"/>.
    /// </summary>
    public abstract class IndustrialAppStoreHttpFactory : IIndustrialAppStoreHttpFactory {

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
        protected virtual HttpMessageHandler CreateHandler() => _httpMessageHandlerFactory.CreateHandler(nameof(IndustrialAppStoreHttpClient));


        /// <summary>
        /// Creates a new <see cref="HttpClient"/> instance.
        /// </summary>
        /// <returns>
        ///   The <see cref="HttpClient"/> instance.
        /// </returns>
        protected virtual HttpClient CreateClient() {
            var http = new HttpClient(CreateHandler(), false);
#if NET8_0_OR_GREATER
            http.DefaultRequestVersion = System.Net.HttpVersion.Version11;
            http.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
#endif
            return http;
        }


        /// <inheritdoc/>
        HttpMessageHandler IIndustrialAppStoreHttpFactory.CreateHandler() => CreateHandler();


        /// <inheritdoc/>
        HttpClient IIndustrialAppStoreHttpFactory.CreateClient() => CreateClient();

    }
}
