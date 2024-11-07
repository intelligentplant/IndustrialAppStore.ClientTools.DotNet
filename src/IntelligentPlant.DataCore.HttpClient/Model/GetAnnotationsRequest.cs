using Newtonsoft.Json;

namespace IntelligentPlant.DataCore.Client.Model {
    /// <summary>
    /// POST-ed annotation data request.
    /// </summary>
    public class GetAnnotationsRequest {

        /// <summary>
        /// Gets or sets the data source name.
        /// </summary>
        [JsonProperty("dsn")]
        public string Dsn { get; set; } = default!;

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        [JsonProperty("tags")]
        public IEnumerable<string> Tags { get; set; } = default!;

        /// <summary>
        /// Gets or sets the query start time.
        /// </summary>
        [JsonProperty("start")]
        public string Start { get; set; } = default!;

        /// <summary>
        /// Gets or sets the query start time.
        /// </summary>
        [JsonProperty("end")]
        public string End { get; set; } = default!;

    }
}
