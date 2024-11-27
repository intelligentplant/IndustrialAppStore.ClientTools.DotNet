using System.Text.Json.Serialization;

#nullable disable warnings

namespace IntelligentPlant.IndustrialAppStore.CommandLine.OAuth {

    /// <summary>
    /// Describes an OAuth error response.
    /// </summary>
    public class OAuthErrorResponse {

        /// <summary>
        /// The error code.
        /// </summary>
        [JsonPropertyName("error")]
        public string Error { get; }

        /// <summary>
        /// The error description.
        /// </summary>
        [JsonPropertyName("error_description")]
        public string? ErrorDescription { get; }

        /// <summary>
        /// The error URI.
        /// </summary>
        [JsonPropertyName("error_uri")]
        public Uri? ErrorUri { get; }


        /// <summary>
        /// Creates a new <see cref="OAuthErrorResponse"/> instance.
        /// </summary>
        /// <param name="error">
        ///   The error code.
        /// </param>
        /// <param name="errorDescription">
        ///   The error description.
        /// </param>
        /// <param name="errorUri">
        ///   The error URI.
        /// </param>
        public OAuthErrorResponse(string error, string? errorDescription, Uri? errorUri) {
            Error = error;
            ErrorDescription = errorDescription;
            ErrorUri = errorUri;
        }

    }
}
