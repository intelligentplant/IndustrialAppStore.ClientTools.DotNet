using System;
using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.DataCore.Client {

    /// <summary>
    /// Options for <see cref="DataCoreHttpClient"/>.
    /// </summary>
    public class DataCoreHttpClientOptions {

        /// <summary>
        /// The base URL for Data Core API calls.
        /// </summary>
        [Required]
        public Uri DataCoreUrl { get; set; } = default!;

    }
}
