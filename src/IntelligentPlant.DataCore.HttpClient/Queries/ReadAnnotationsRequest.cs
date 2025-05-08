using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.DataCore.Client.Queries {

    /// <summary>
    /// Describes a request to read annotations.
    /// </summary>
    public class ReadAnnotationsRequest : IValidatableObject {

        /// <summary>
        /// The data source to query.
        /// </summary>
        [Required]
        [Newtonsoft.Json.JsonProperty("dsn")]
        [System.Text.Json.Serialization.JsonPropertyName("dsn")]
        public string DataSourceName { get; set; } = default!;

        /// <summary>
        /// The tags to request annotations from.
        /// </summary>
        [Required]
        [MinLength(1)]
        [Newtonsoft.Json.JsonProperty("tags")]
        [System.Text.Json.Serialization.JsonPropertyName("tags")]
        public string[] TagNames { get; set; } = default!;

        /// <summary>
        /// The UTC start time for the query.
        /// </summary>
        [Required]
        [Newtonsoft.Json.JsonProperty("start")]
        [System.Text.Json.Serialization.JsonPropertyName("start")]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// The UTC end time for the query.
        /// </summary>
        [Required]
        [Newtonsoft.Json.JsonProperty("end")]
        [System.Text.Json.Serialization.JsonPropertyName("end")]
        public DateTime EndTime { get; set; }


        /// <inheritdoc/>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (StartTime > EndTime) {
                yield return new ValidationResult(Resources.Error_StartTimeCannotBeGreaterThanEndTime, new[] { nameof(StartTime) });
            }
        }

    }

}
