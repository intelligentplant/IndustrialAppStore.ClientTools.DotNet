using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace IntelligentPlant.DataCore.Client {

    /// <summary>
    /// An RFC 7807 problem details object.
    /// </summary>
    public class ProblemDetails {

        /// <summary>
        /// Media type for a JSON-encoded problem details object.
        /// </summary>
        public const string JsonMediaType = "application/problem+json";

        /// <summary>
        /// A URI reference that identifies the problem type.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// A short, human-readable summary of the problem.
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; }

        /// <summary>
        /// The HTTP status code for the problem.
        /// </summary>
        [JsonPropertyName("status")]
        public int? Status { get; set; }

        /// <summary>
        /// A human-readable explanation of the problem.
        /// </summary>
        [JsonPropertyName("detail")]
        public string Detail { get; set; }

        /// <summary>
        /// A URI reference that identifies the specific occurrence of the problem.
        /// </summary>
        [JsonPropertyName("instance")]
        public string Instance { get; set; }

        /// <summary>
        /// A dictionary of extension members for the problem.
        /// </summary>
        /// <remarks>
        ///   Problem type definitions may extend the problem details object with additional 
        ///   members. When the object is deserialized, these additonal members are added to 
        ///   this dictionary.
        /// </remarks>
        [JsonExtensionData]
        public IDictionary<string, object> Extensions { get; set; }

    }
}
