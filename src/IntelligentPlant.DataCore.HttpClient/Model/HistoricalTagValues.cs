using System.Collections.Generic;
using System.Linq;

namespace IntelligentPlant.DataCore.Client.Model
{
    /// <summary>
    /// Describes the results of a historical data query for a single tag.
    /// </summary>
    public class HistoricalTagValues {

        /// <summary>
        /// Gets or sets the tag name that the <see cref="Values"/> are for.
        /// </summary>
        public string TagName { get; set; }

        /// <summary>
        /// Gets or sets the display type to use when visualizing the <see cref="Values"/>.
        /// </summary>
        public HistoricalTagValuesDisplayType DisplayType { get; set; }

        /// <summary>
        /// The data samples.
        /// </summary>
        private TagValue[] _values = new TagValue[0];

        /// <summary>
        /// Gets or sets the data samples.
        /// </summary>
        public IEnumerable<TagValue> Values {
            get { return _values; }
            set { _values = value?.ToArray() ?? new TagValue[0]; }
        }
		
    }


    /// <summary>
    /// Describes how the values in a <see cref="HistoricalTagValues"/> set should be visualized.  
    /// This is a hint for applications that will trend data on-screen.
    /// </summary>
    public enum HistoricalTagValuesDisplayType {

        /// <summary>
        /// The display type is not specified.
        /// </summary>
        Unspecified,

        /// <summary>
        /// Interpolation should be performed between samples.  This is the recommended value for <see cref="DataFunctions.Interpolated"/> 
        /// and <see cref="DataFunctions.Plot"/> queries on analogue tags.
        /// </summary>
        Interpolate,

        /// <summary>
        /// No interpolation should be performed between samples.  The step change from one value to 
        /// another should occur as a vertical line at the sample time of the value change.  This is 
        /// the recommended value for digital or state-based tags, and also for analogue tags when 
        /// querying for <see cref="DataFunctions.Raw"/> data, or aggregated data calculated 
        /// at a set interval (e.g. <see cref="DataFunctions.Average"/>).
        /// </summary>
        TrailingEdge,

        /// <summary>
        /// No interpolation should be performed between samples.  The step change from one value to 
        /// another should occur as a vertical line at the sample time *prior* to the value change.
        /// </summary>
        LeadingEdge

    }
}
