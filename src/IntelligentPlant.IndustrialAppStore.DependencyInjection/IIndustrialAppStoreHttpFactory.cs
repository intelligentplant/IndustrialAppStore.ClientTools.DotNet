using System.Net.Http;

namespace IntelligentPlant.IndustrialAppStore.DependencyInjection {

    /// <summary>
    /// <see cref="IIndustrialAppStoreHttpFactory"/> is responsible for creating HTTP handlers for 
    /// use with <see cref="Client.IndustrialAppStoreHttpClient"/> instances.
    /// </summary>
    /// <remarks>
    /// 
    /// <para>
    ///   <see cref="IIndustrialAppStoreHttpFactory"/> allows the HTTP request pipeline for a 
    ///   client to be customised in ways that may not be possible when using <see cref="IHttpMessageHandlerFactory"/> 
    ///   directly, such as adding custom scope-specific authentication headers to outgoing 
    ///   requests.
    /// </para>
    /// 
    /// <para>
    ///   Implementations should inherit from <see cref="IndustrialAppStoreHttpFactory"/> instead 
    ///   of implementing <see cref="IIndustrialAppStoreHttpFactory"/> directly.
    /// </para>
    /// 
    /// </remarks>
    public interface IIndustrialAppStoreHttpFactory {

        /// <summary>
        /// Creates a new HTTP message handler.
        /// </summary>
        /// <returns>
        ///   The HTTP message handler.
        /// </returns>
        HttpMessageHandler CreateHandler();

        /// <summary>
        /// Creates a new <see cref="HttpClient"/> instance.
        /// </summary>
        /// <returns>
        ///   The <see cref="HttpClient"/> instance.
        /// </returns>
        HttpClient CreateClient();

    }

}
