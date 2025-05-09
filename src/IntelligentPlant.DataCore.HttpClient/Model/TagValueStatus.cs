namespace IntelligentPlant.DataCore.Client.Model {
    /// <summary>
    /// Describes the quality status for a tag value.
    /// </summary>
    [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
    public enum TagValueStatus {
        /// <summary>
        /// The value has bad quality.
        /// </summary>
        Bad = 0,
        /// <summary>
        /// The value has uncertain/questionable quality.
        /// </summary>
        Uncertain = 64,
        /// <summary>
        /// The value has good quality.
        /// </summary>
        Good = 192
    }
}
