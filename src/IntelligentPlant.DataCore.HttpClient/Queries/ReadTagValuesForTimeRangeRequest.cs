using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IntelligentPlant.DataCore.Client.Queries {

    /// <summary>
    /// Base class for requests that query tags over a time range.
    /// </summary>
    public abstract class ReadTagValuesForTimeRangeRequest : ReadTagValuesRequest {

        /// <summary>
        /// The UTC start time for the query.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// The UTC end time for the query.
        /// </summary>
        public DateTime EndTime { get; set; }


        /// <inheritdoc/>
        protected override IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            foreach (var item in base.Validate(validationContext)) {
                yield return item;
            }

            if (StartTime > EndTime) {
                yield return new ValidationResult(Resources.Error_StartTimeCannotBeGreaterThanEndTime, new[] { nameof(StartTime) });
            }
        }

    }
}
