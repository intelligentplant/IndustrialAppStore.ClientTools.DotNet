using System;

namespace IntelligentPlant.DataCore.Client.Model {
    /// <summary>
    /// Describes a tag value that will be written to a data source.
    /// </summary>
    public class TagValueUpdate {

        /// <summary>
        /// Gets or sets the UTC timestamp for the sample.
        /// </summary>
        public DateTime UtcSampleTime { get; set; }

        /// <summary>
        /// Gets or sets the sample value.
        /// </summary>
        public object Value { get; set; } = default!;

        /// <summary>
        /// Gets or sets the sample quality status.
        /// </summary>
        public TagValueStatus Status { get; set; }

    }
}
