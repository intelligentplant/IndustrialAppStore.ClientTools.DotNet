using System.Net.Http;

namespace IntelligentPlant.IndustrialAppStore.DependencyInjection {

    /// <summary>
    /// Default implementation of <see cref="IIndustrialAppStoreHttpFactory"/>.
    /// </summary>
    public sealed class DefaultIndustrialAppStoreHttpFactory : IndustrialAppStoreHttpFactory {

        /// <summary>
        /// Creates a new <see cref="DefaultIndustrialAppStoreHttpFactory"/> instance.
        /// </summary>
        /// <param name="httpMessageHandlerFactory">
        ///   The HTTP message handler factory.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        ///   <paramref name="httpMessageHandlerFactory"/> is <see langword="null"/>.
        /// </exception>
        public DefaultIndustrialAppStoreHttpFactory(IHttpMessageHandlerFactory httpMessageHandlerFactory)
            : base(httpMessageHandlerFactory) { }

    }

}
