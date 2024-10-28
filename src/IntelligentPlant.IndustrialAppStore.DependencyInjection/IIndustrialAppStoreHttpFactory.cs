using System.Net.Http;

namespace IntelligentPlant.IndustrialAppStore.DependencyInjection {

    /// <summary>
    /// <see cref="IIndustrialAppStoreHttpFactory"/> is responsible for creating HTTP handlers for 
    /// use with <see cref="Client.IndustrialAppStoreHttpClient"/> instances.
    /// </summary>
    /// <remarks>
    ///   <see cref="IIndustrialAppStoreHttpFactory"/> allows the HTTP request pipeline for a 
    ///   client to be customised in ways that may not be possible when using <see cref="IHttpMessageHandlerFactory"/> 
    ///   directly, such as adding custom scope-specific authentication headers to outgoing 
    ///   requests.
    /// </remarks>
    public interface IIndustrialAppStoreHttpFactory {

        /// <summary>
        /// Creates a new HTTP message handler.
        /// </summary>
        /// <returns>
        ///   The HTTP message handler.
        /// </returns>
        HttpMessageHandler CreateHandler();

    }

}
