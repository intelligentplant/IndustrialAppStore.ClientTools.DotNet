using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

#nullable disable

namespace IntelligentPlant.DataCore.Client.Model {
    /// <summary>
    /// Describes a data query result for a single tag, before it has been transformed.
    /// </summary>
    [Obsolete("DO NOT USE!")]
    public class IntermediateTagValues : IIntermediateTagValues {
        /// <summary>
        /// Gets or sets the tag ID.
        /// </summary>
        public int tagId { get; set; }

        /// <summary>
        /// Gets or sets the tag name.
        /// </summary>
        public string tagName { get; set; }

        /// <summary>
        /// Gets or sets the data source name.
        /// </summary>
        public string dataSourceName { get; set; }

        /// <summary>
        /// Gets or sets the friendly name for the tag.
        /// </summary>
        public string tagFriendlyName { get; set; }

        /// <summary>
        /// Gets or sets the tag units.
        /// </summary>
        public string tagUnits { get; set; }

        /// <summary>
        /// Gets or sets a flag specifying if the tag is a digital tag or not.
        /// </summary>
        public bool isDigital { get; set; }

        /// <summary>
        /// Gets or sets the available states for the tag.
        /// </summary>
        public IEnumerable<string> states { get; set; }

        /// <summary>
        /// Gets or sets the numerical data for the data query.
        /// </summary>
        public IEnumerable<double> tagData { get; set; }

        /// <summary>
        /// Gets or sets the time stamps for the data query.
        /// </summary>
        [JsonProperty(ItemConverterType = typeof(IsoDateTimeConverter))]
        public IEnumerable<DateTime> tagDataTime { get; set; }

        /// <summary>
        /// Gets or sets the digital state values for the data query.
        /// </summary>
        /// <remarks>
        /// When returning digital state data from a historian, state names should be returned in this property, while 
        /// <see cref="tagData"/> should be used to return the equivalent numerical value for the state.
        /// </remarks>
        public IEnumerable<string> tagStringValues { get; set; }

        /// <summary>
        /// Gets or sets the status values for the samples.
        /// </summary>
        public IEnumerable<TagValueStatus> statusValues { get; set; }

        /// <summary>
        /// Gets or sets the components of the original data query.
        /// </summary>
        public IEnumerable<string> originalQuery { get; set; }

        /// <summary>
        /// Gets or sets the query start time.
        /// </summary>
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime startTime { get; set; }

        /// <summary>
        /// Gets or sets the query end time.
        /// </summary>
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime endTime { get; set; }

        /// <summary>
        /// Gets or sets the minimum numerical value for the query.
        /// </summary>
        public double maximumDataValue { get; set; }

        /// <summary>
        /// Gets or sets the maximum numerical value for the query.
        /// </summary>
        public double minimumDataValue { get; set; }

        /// <summary>
        /// Gets or sets the upper normal operating limit for the tag.
        /// </summary>
        public double NOLUpper { get; set; }

        /// <summary>
        /// Gets or sets the lower normal operating limit for the tag.
        /// </summary>
        public double NOLLower { get; set; }

        /// <summary>
        /// Gets or sets the upper safe operating limit for the tag.
        /// </summary>
        public double SOLUpper { get; set; }

        /// <summary>
        /// Gets or sets the lower safe operating limit for the tag.
        /// </summary>
        public double SOLLower { get; set; }

        /// <summary>
        /// Gets or sets the upper safe design limit for the tag.
        /// </summary>
        public double SDLUpper { get; set; }

        /// <summary>
        /// Gets or sets the lower safe design limit for the tag.
        /// </summary>
        public double SDLLower { get; set; }

        /// <summary>
        /// Gets or sets the result status.
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// Gets or sets a flag specifying if the client should perform additional interpolation on the query results, or if they have been pre-interpolated.
        /// </summary>
        /// <remarks>
        /// <see langword="true"/> indicates that the client should perform additional interpolation on the response.
        /// </remarks>
        public bool interp { get; set; }


        /// <summary>
        /// Creates a new <see cref="IntermediateTagValues"/> object.
        /// </summary>
        public IntermediateTagValues() {
            interp = true;
            states = new List<string>();
            tagDataTime = new List<DateTime>();
            tagData = new List<double>();
            tagStringValues = new List<string>();
            statusValues = new List<TagValueStatus>();
            originalQuery = new List<string>();
        }

    }
}
