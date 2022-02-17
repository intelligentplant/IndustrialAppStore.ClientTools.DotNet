using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

namespace IntelligentPlant.DataCore.Client.Model.Scripting {

    /// <summary>
    /// Describes the value script settings for a script tag.
    /// </summary>
    public class ScriptTagValueScriptSettings: IValidatableObject {

        /// <summary>
        /// Gets or sets the script to run when evaluating the tag value.
        /// </summary>
        [Required]
        [MaxLength(2000)]
        public string Script { get; set; } = default!;

        /// <summary>
        /// The trigger type used to trigger recalculation.
        /// </summary>
        public ScriptTagTriggerType TriggerType { get; set; }

        /// <summary>
        /// When <see cref="TriggerType"/> is <see cref="ScriptTagTriggerType.Schedule"/>, 
        /// this is the CRON schedule to recalculate the script tag's value at. 
        /// </summary>
        [MaxLength(100)]
        public string? Schedule { get; set; }

        /// <summary>
        /// Describes the type of input data that will be retrieved for tag references when 
        /// recalculating the script tag's value.
        /// </summary>
        public ScriptTagInputDataType InputDataType { get; set; }

        /// <summary>
        /// When <see cref="InputDataType"/> is <see cref="ScriptTagInputDataType.AggregatedValues"/>, 
        /// these are the aggregate data functions that will be used when requesting input data for 
        /// the script tag. <see cref="DataFunctions.CurrentValue"/>, <see cref="DataFunctions.Raw"/> 
        /// and <see cref="DataFunctions.Plot"/> cannot be specified here.
        /// </summary>
        public string[]? InputDataFunctions { get; set; }

        /// <summary>
        /// When <see cref="InputDataType"/> is <see cref="ScriptTagInputDataType.AggregatedValues"/>, 
        /// this is the sample interval used when requesting aggregated input data for the script tag.
        /// </summary>
        public string? InputDataSampleInterval { get; set; }

        /// <summary>
        /// This is the batch size to use when requesting input data for the script tag. For example, 
        /// if the batch size is 5 days, and catch-up is being performed to recalculate the script 
        /// tag value from 30 days ago until the current time, the input data would be requested and 
        /// processed in six 5-day batches.
        /// </summary>
        public string InputDataBlockSize { get; set; } = "12h";

        /// <summary>
        /// Gets the data window size to use when requesting data for the value script. When greater 
        /// than zero, each evaluation of the script will be passed an array of values for any 
        /// referenced tags that represent the window leading up to the evaluation sample time, 
        /// instead of just the referenced tag values at the evaluation sample time.
        /// </summary>
        public string? InputDataWindowSize { get; set; }

        /// <summary>
        /// Gets or sets a flag specifying if snapshot recalculations for the script tag should be 
        /// recorded for future inspection. The script host ultimately decides if this hint will be 
        /// followed, where the history is recorded to, and how long it is stored for.
        /// </summary>
        public bool RecordSnapshotRecalculation { get; set; }


        /// <summary>
        /// Validates the object.
        /// </summary>
        /// <param name="validationContext">
        ///   The validation context.
        /// </param>
        /// <returns>
        ///   The validation errors.
        /// </returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (TriggerType == ScriptTagTriggerType.Schedule) {
                if (String.IsNullOrWhiteSpace(Schedule)) {
                    yield return new ValidationResult(Resources.Error_Scripting_ScheduleIsRequired, new[] { nameof(Schedule) });
                }
            }

            if (InputDataType == ScriptTagInputDataType.AggregatedValues) {
                if (InputDataFunctions?.Length < 1) {
                    yield return new ValidationResult(Resources.Error_Scripting_AggregatedFunctionListCannotBeEmpty, new[] { nameof(InputDataFunctions) });
                }
                if (InputDataFunctions.Any(x => String.IsNullOrWhiteSpace(x))) {
                    yield return new ValidationResult(Resources.Error_Scripting_AggregatedFunctionListCannotContainEmptyValues, new[] { nameof(InputDataFunctions) });
                }
                if (InputDataFunctions.Contains(DataFunctions.Raw, StringComparer.OrdinalIgnoreCase) || InputDataFunctions.Contains(DataFunctions.Plot, StringComparer.OrdinalIgnoreCase) || InputDataFunctions.Contains(DataFunctions.CurrentValue, StringComparer.OrdinalIgnoreCase)) {
                    yield return new ValidationResult(String.Format(CultureInfo.CurrentCulture, Resources.Error_Scripting_AggregatedFunctionListCanContainOnlyAggregateFunctions, String.Join(", ", new[] { DataFunctions.Raw, DataFunctions.Plot, DataFunctions.CurrentValue })), new[] { nameof(InputDataFunctions) });
                }

                if (string.IsNullOrWhiteSpace(InputDataSampleInterval) || !IntelligentPlant.Relativity.RelativityParser.Default.TryConvertToTimeSpan(InputDataSampleInterval, out var _)) {
                    yield return new ValidationResult(Resources.Error_Scripting_InvalidTimeSpan, new[] { nameof(InputDataSampleInterval) });
                }
            }

            if (string.IsNullOrWhiteSpace(InputDataBlockSize) || !IntelligentPlant.Relativity.RelativityParser.Default.TryConvertToTimeSpan(InputDataBlockSize, out var _)) {
                yield return new ValidationResult(Resources.Error_Scripting_InvalidTimeSpan, new[] { nameof(InputDataBlockSize) });
            }
        }
    }
}
