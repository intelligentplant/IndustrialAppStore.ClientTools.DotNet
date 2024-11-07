using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace IntelligentPlant.DataCore.Client.Model {
    /// <summary>
    /// Describes a tag value.
    /// </summary>
    public class TagValue {

        /// <summary>
        /// Gets the name of the tag.
        /// </summary>
        public string TagName { get; private set; }

        /// <summary>
        /// Gets the UTC sample time.
        /// </summary>
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime UtcSampleTime { get; private set; }

        /// <summary>
        /// Gets the numeric value for the tag.
        /// </summary>
        /// <remarks>
        /// For digital tags, this field returns the value of the digital state.
        /// </remarks>
        public double NumericValue { get; private set; }

        /// <summary>
        /// Gets the text value for the tag.
        /// </summary>
        /// <remarks>
        /// For digital tags, this field returns the name of the digital state.
        /// </remarks>
        public string? TextValue { get; private set; }

        /// <summary>
        /// Gets the quality status for the value.
        /// </summary>
        public TagValueStatus Status { get; private set; }

        /// <summary>
        /// Gets the unit of measure for the tag value.
        /// </summary>
        public string? Unit { get; private set; }

        /// <summary>
        /// Gets notes associated with the value.
        /// </summary>
        public string? Notes { get; private set; }

        /// <summary>
        /// Gets the error message associated with the value.
        /// </summary>
        public string? Error { get; private set; }

        /// <summary>
        /// Gets a flag indicating if the tag value contains a numeric value.
        /// </summary>
        public bool IsNumeric {
            get { return !double.IsNaN(NumericValue); }
        }

        /// <summary>
        /// Gets a flag indicating if the tag value contains an error message in the <see cref="Error"/> property.
        /// </summary>
        public bool HasError {
            get { return !string.IsNullOrWhiteSpace(Error); }
        }


        /// <summary>
        /// Creates a new <see cref="TagValue"/> object.
        /// </summary>
        /// <param name="tagName">The tag name.</param>
        /// <param name="utcSampleTime">The UTC sample time.</param>
        /// <param name="numericValue">The numeric value.</param>
        /// <param name="textValue">The text value.</param>
        /// <param name="status">The quality status for the value.</param>
        /// <param name="unit">The unit of measure for the tag value.</param>
        /// <param name="notes">Additional notes about the value.</param>
        /// <param name="error">An error message associated with the value.</param>
        [JsonConstructor]
        public TagValue(string tagName, DateTime utcSampleTime, double numericValue, string? textValue, TagValueStatus status, string? unit, string? notes, string? error) {
            TagName = tagName;
            UtcSampleTime = utcSampleTime;
            NumericValue = numericValue;
            TextValue = textValue;
            Status = status;
            Unit = unit;
            Notes = notes;
            Error = error;
        }


        /// <summary>
        /// Creates a new <see cref="TagValue"/> object.
        /// </summary>
        /// <param name="tagName">The tag name.</param>
        /// <param name="utcSampleTime">The UTC sample time.</param>
        /// <param name="numericValue">The numeric value.</param>
        /// <param name="textValue">The text value.</param>
        /// <param name="status">The quality status for the value.</param>
        /// <param name="unit">The unit of measure for the tag value.</param>
        /// <param name="notes">Additional notes about the value.</param>
        public TagValue(string tagName, DateTime utcSampleTime, double numericValue, string? textValue, TagValueStatus status, string? unit, string? notes) : this(tagName, utcSampleTime, numericValue, textValue, status, unit, notes, null) { }



        /// <summary>
        /// Creates a new <see cref="TagValue"/> object.
        /// </summary>
        /// <param name="tagName">The tag name.</param>
        /// <param name="utcSampleTime">The UTC sample time.</param>
        /// <param name="numericValue">The numeric value.</param>
        /// <param name="textValue">The text value.</param>
        /// <param name="status">The quality status for the value.</param>
        /// <param name="unit">The unit of measure for the tag value.</param>
        public TagValue(string tagName, DateTime utcSampleTime, double numericValue, string? textValue, TagValueStatus status, string? unit) : this(tagName, utcSampleTime, numericValue, textValue, status, unit, null) { }


        /// <summary>
        /// Creates a string representation of the value.
        /// </summary>
        /// <returns>
        /// A string representation of the value.
        /// </returns>
        public override string ToString() {
            var builder = new StringBuilder();
            if (string.IsNullOrWhiteSpace(TextValue)) {
                builder.Append(NumericValue);
            }
            else {
                builder.Append(TextValue);
            }

            if (!string.IsNullOrWhiteSpace(Unit)) {
                builder.Append(' ');
                builder.Append(Unit);
            }

            builder.Append(" @ ");
            builder.Append(UtcSampleTime.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
            builder.Append(" (");
            builder.Append(Status.ToString().ToLowerInvariant());
            builder.Append(" quality)");

            return builder.ToString();
        }
    }
}
