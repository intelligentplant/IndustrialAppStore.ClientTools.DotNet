namespace IntelligentPlant.DataCore.Client.Model.Scripting {
    /// <summary>
    /// Describes which property of a <see cref="TagValue"/> object should be 
    /// passed into an evaluation script when defining a <see cref="TagReference"/>.
    /// </summary>
    [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
    public enum TagReferenceType {
        /// <summary>
        /// The reference maps to the <see cref="TagValue.NumericValue"/> property of a tag value.
        /// </summary>
        NumericValue,
        /// <summary>
        /// The reference maps to the <see cref="TagValue.TextValue"/> property of a tag value.
        /// </summary>
        TextValue,
        /// <summary>
        /// The reference maps to the <see cref="TagValue.UtcSampleTime"/> property of a tag value.
        /// </summary>
        TimeStamp,
        /// <summary>
        /// The reference maps to the <see cref="TagValue.Status"/> property of a tag value.
        /// </summary>
        Status,
        /// <summary>
        /// The reference maps to the <see cref="TagValue.Unit"/> property of a tag value.
        /// </summary>
        Unit,
        /// <summary>
        /// The reference maps to the full <see cref="TagValue"/> object.
        /// </summary>
        Object
    }
}
