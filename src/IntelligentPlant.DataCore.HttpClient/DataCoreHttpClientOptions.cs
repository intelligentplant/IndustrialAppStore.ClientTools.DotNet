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

        /// <summary>
        /// The JSON serializer to use when serializing and deserializing requests and responses.
        /// </summary>
        public JsonSerializerType JsonSerializer { get; set; } = JsonSerializerType.Newtonsoft;

        /// <summary>
        /// The JSON serializer options to use when using the System.Text.Json serializer.
        /// </summary>
        public System.Text.Json.JsonSerializerOptions? JsonOptions { get; set; }

    }


    /// <summary>
    /// Specifies the JSON serializer to use when serializing and deserializing requests and responses.
    /// </summary>
    public enum JsonSerializerType {

        /// <summary>
        /// Newtonsoft.Json (JSON.NET) serializer.
        /// </summary>
        Newtonsoft,

        /// <summary>
        /// System.Text.Json serializer.
        /// </summary>
        SystemTextJson

    }

}
