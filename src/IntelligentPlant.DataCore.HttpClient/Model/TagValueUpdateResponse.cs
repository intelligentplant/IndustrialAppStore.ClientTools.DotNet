namespace IntelligentPlant.DataCore.Client.Model {
    /// <summary>
    /// Describes the response sent by Data Core after writing tag values to a data source.
    /// </summary>
    public class TagValueUpdateResponse {

        /// <summary>
        /// Gets or sets the tag name.
        /// </summary>
        public string TagName { get; set; } = default!;

        /// <summary>
        /// Gets or sets a flag indicating if the write was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets a collection of messages associated with the write.
        /// </summary>
        public ICollection<string>? Messages { get; set; }

    }
}
