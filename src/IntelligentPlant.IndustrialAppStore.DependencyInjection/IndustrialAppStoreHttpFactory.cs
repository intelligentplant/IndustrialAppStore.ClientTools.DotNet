using System;
using System.Net.Http;

using IntelligentPlant.IndustrialAppStore.Client;

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


        /// <inheritdoc/>
        public virtual HttpMessageHandler CreateHandler() => _httpMessageHandlerFactory.CreateHandler(nameof(IndustrialAppStoreHttpClient));

    }
}
