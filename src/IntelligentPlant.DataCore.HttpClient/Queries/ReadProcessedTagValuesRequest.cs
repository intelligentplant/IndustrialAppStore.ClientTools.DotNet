using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.DataCore.Client.Queries {

    /// <summary>
    /// A query for processed tag values (e.g. interpolated, average, minimum, or maximum).
    /// </summary>
    public class ReadProcessedTagValuesRequest : ReadTagValuesForTimeRangeRequest {

        /// <summary>
        /// The data processing function to use. See <see cref="DataFunctions"/> for 
        /// commonly-supported function names.
        /// </summary>
        [Required]
        public string DataFunction { get; set; }

        /// <summary>
        /// The sample interval to use when processing the tag values.
        /// </summary>
        public TimeSpan SampleInterval { get; set; }


        /// <inheritdoc/>
        protected override IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            foreach (var item in base.Validate(validationContext)) {
                yield return item;
            }

            if (SampleInterval <= TimeSpan.Zero) {
                yield return new ValidationResult(Resources.Error_PositiveSampleIntervalIsRequired, new[] { nameof(SampleInterval) });
            }
        }

    }
}
