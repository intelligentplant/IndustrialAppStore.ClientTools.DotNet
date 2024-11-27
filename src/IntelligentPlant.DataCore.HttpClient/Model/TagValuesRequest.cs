using Newtonsoft.Json;

#nullable disable

namespace IntelligentPlant.DataCore.Client.Model {
    /// <summary>
    /// Describes a tag values request to be sent to Data Core.
    /// </summary>
    [Obsolete("DO NOT USE!")]
    public class TagValuesRequest {

        /// <summary>
        /// Gets or sets the tags in the request.
        /// </summary>
        [JsonProperty("tags")]
        public IEnumerable<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets the data source names for each element in <see cref="Tags"/>.
        /// </summary>
        [JsonProperty("dataSourceNames")]
        public IEnumerable<string> DataSourceNames { get; set; }

        /// <summary>
        /// Gets or sets the absolute or relative start time for the request.
        /// </summary>
        [JsonProperty("startTime")]
        public string StartTime { get; set; }

        /// <summary>
        /// Gets or sets the absolute or relative end time for the request.
        /// </summary>
        [JsonProperty("endTime")]
        public string EndTime { get; set; }

        /// <summary>
        /// Gets or sets the sample interval for the request.
        /// </summary>
        [JsonProperty("timeStep")]
        public string SampleInterval { get; set; }

        /// <summary>
        /// Gets or sets the number of samples to request
        /// </summary>
        [JsonProperty("points")]
        public int SampleCount { get; set; }

        /// <summary>
        /// Gets or sets the data function to use (e.g. INTERP, RAW, NOW, AVG, MAX, PLOT, STDDEV).
        /// </summary>
        [JsonProperty("quantise")]
        public string DataFunction { get; set; }

    }
}
