using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace IntelligentPlant.DataCore.Client.Queries {

    /// <summary>
    /// Describes a request to read annotations.
    /// </summary>
    public class ReadAnnotationsRequest : IValidatableObject {

        /// <summary>
        /// The data source to query.
        /// </summary>
        [Required]
        [JsonPropertyName("dsn")]
        public string DataSourceName { get; set; }

        /// <summary>
        /// The tags to request annotations from.
        /// </summary>
        [Required]
        [MinLength(1)]
        [JsonPropertyName("tags")]
        public string[] TagNames { get; set; }

        /// <summary>
        /// The UTC start time for the query.
        /// </summary>
        [Required]
        [JsonPropertyName("start")]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// The UTC end time for the query.
        /// </summary>
        [Required]
        [JsonPropertyName("end")]
        public DateTime EndTime { get; set; }


        /// <inheritdoc/>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (StartTime > EndTime) {
                yield return new ValidationResult(Resources.Error_StartTimeCannotBeGreaterThanEndTime, new[] { nameof(StartTime) });
            }
        }

    }

}
