using System;
using System.Net.Http;

using IntelligentPlant.DataCore.Client.Clients;

namespace IntelligentPlant.IndustrialAppStore.Client.Clients {

    /// <summary>
    /// Base class for IAS client sub-types.
    /// </summary>
    public abstract class IasClientBase : ClientBase<IndustrialAppStoreHttpClientOptions> {

        /// <inheritdoc/>
        protected override Uri BaseUrl {
            get { return Options.AppStoreUrl; }
        }


        /// <summary>
        /// Creates a new <see cref="IasClientBase"/> object.
        /// </summary>
        /// <param name="httpClient">
        ///   The HTTP client to use.
        /// </param>
        /// <param name="options">
        ///   The HTTP client options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="httpClient"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   <paramref name="options"/> is <see langword="null"/>.
        /// </exception>
        protected IasClientBase(HttpClient httpClient, IndustrialAppStoreHttpClientOptions options)
            : base(httpClient, options) { }

    }
}
