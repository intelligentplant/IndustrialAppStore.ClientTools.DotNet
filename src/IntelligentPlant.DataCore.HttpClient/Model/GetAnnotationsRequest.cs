namespace IntelligentPlant.DataCore.Client.Model {
    /// <summary>
    /// POST-ed annotation data request.
    /// </summary>
    public class GetAnnotationsRequest {

        /// <summary>
        /// Gets or sets the data source name.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("dsn")]
        [System.Text.Json.Serialization.JsonPropertyName("dsn")]
        public string Dsn { get; set; } = default!;

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("tags")]
        [System.Text.Json.Serialization.JsonPropertyName("tags")]
        public IEnumerable<string> Tags { get; set; } = default!;

        /// <summary>
        /// Gets or sets the query start time.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("start")]
        [System.Text.Json.Serialization.JsonPropertyName("start")]
        public string Start { get; set; } = default!;

        /// <summary>
        /// Gets or sets the query start time.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("end")]
        [System.Text.Json.Serialization.JsonPropertyName("end")]
        public string End { get; set; } = default!;

    }
}
