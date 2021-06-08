using System;
using System.Text.Json;

namespace IntelligentPlant.DataCore.Client {

    /// <summary>
    /// Options for <see cref="DataCoreHttpClient"/>.
    /// </summary>
    public class DataCoreHttpClientOptions {

        /// <summary>
        /// The base URL for Data Core API calls.
        /// </summary>
        public Uri DataCoreUrl { get; set; }

        /// <summary>
        /// The JSON options for the client.
        /// </summary>
        public JsonSerializerOptions JsonOptions { get; set; }

    }
}
