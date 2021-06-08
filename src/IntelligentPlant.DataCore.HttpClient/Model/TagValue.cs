using System;
using System.Text;

namespace IntelligentPlant.DataCore.Client.Model {
    /// <summary>
    /// Describes a tag value.
    /// </summary>
    public class TagValue {

        /// <summary>
        /// Gets the name of the tag.
        /// </summary>
        public string TagName { get; set; }

        /// <summary>
        /// Gets the UTC sample time.
        /// </summary>
        public DateTime UtcSampleTime { get; set; }

        /// <summary>
        /// Gets the numeric value for the tag.
        /// </summary>
        /// <remarks>
        /// For digital tags, this field returns the value of the digital state.
        /// </remarks>
        public double NumericValue { get; set; }

        /// <summary>
        /// Gets the text value for the tag.
        /// </summary>
        /// <remarks>
        /// For digital tags, this field returns the name of the digital state.
        /// </remarks>
        public string TextValue { get; set; }

        /// <summary>
        /// Gets the quality status for the value.
        /// </summary>
        public TagValueStatus Status { get; set; }

        /// <summary>
        /// Gets the unit of measure for the tag value.
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// Gets notes associated with the value.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Gets the error message associated with the value.
        /// </summary>
        public string Error { get; set; }

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
