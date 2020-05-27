using System;
using System.Collections.Generic;

namespace IntelligentPlant.DataCore.Client.Model {

    /// <summary>
    /// Describes a data query result for a single tag returned by Data Core.
    /// </summary>
    public interface IIntermediateTagValues {

        /// <summary>
        /// Gets the tag ID.
        /// </summary>
        int tagId { get; }

        /// <summary>
        /// Gets the tag name.
        /// </summary>
        string tagName { get; }

        /// <summary>
        /// Gets the data source name.
        /// </summary>
        string dataSourceName { get; }

        /// <summary>
        /// Gets the friendly name for the tag.
        /// </summary>
        string tagFriendlyName { get; }

        /// <summary>
        /// Gets the tag units.
        /// </summary>
        string tagUnits { get; }

        /// <summary>
        /// Gets a flag specifying if the tag is a digital tag or not.
        /// </summary>
        bool isDigital { get; }

        /// <summary>
        /// Gets the available states for the tag.
        /// </summary>
        IEnumerable<string> states { get; }

        /// <summary>
        /// Gets the numerical data for the data query.
        /// </summary>
        IEnumerable<double> tagData { get; }

        /// <summary>
        /// Gets the time stamps for the data query.
        /// </summary>
        IEnumerable<DateTime> tagDataTime { get; }

        /// <summary>
        /// Gets the digital state values for the data query.
        /// </summary>
        /// <remarks>
        /// When returning digital state data from a historian, state names should be returned in this property, while 
        /// <see cref="tagData"/> should be used to return the equivalent numerical value for the state.
        /// </remarks>
        IEnumerable<string> tagStringValues { get; }

        /// <summary>
        /// Gets the status values for the samples.
        /// </summary>
        IEnumerable<TagValueStatus> statusValues { get; }

        /// <summary>
        /// Gets the components of the original data query.
        /// </summary>
        IEnumerable<string> originalQuery { get; }

        /// <summary>
        /// Gets the query start time.
        /// </summary>
        DateTime startTime { get; }

        /// <summary>
        /// Gets the query end time.
        /// </summary>
        DateTime endTime { get; }

        /// <summary>
        /// Gets the minimum numerical value for the query.
        /// </summary>
        double maximumDataValue { get; }

        /// <summary>
        /// Gets the maximum numerical value for the query.
        /// </summary>
        double minimumDataValue { get; }

        /// <summary>
        /// Gets the upper normal operating limit for the tag.
        /// </summary>
        double NOLUpper { get; }

        /// <summary>
        /// Gets the lower normal operating limit for the tag.
        /// </summary>
        double NOLLower { get; }

        /// <summary>
        /// Gets the upper safe operating limit for the tag.
        /// </summary>
        double SOLUpper { get; }

        /// <summary>
        /// Gets the lower safe operating limit for the tag.
        /// </summary>
        double SOLLower { get; }

        /// <summary>
        /// Gets the upper safe design limit for the tag.
        /// </summary>
        double SDLUpper { get; }

        /// <summary>
        /// Gets the lower safe design limit for the tag.
        /// </summary>
        double SDLLower { get; }

        /// <summary>
        /// Gets the result status.
        /// </summary>
        string status { get; }

        /// <summary>
        /// Gets a flag specifying if the client should perform additional interpolation on the query results, or if they have been pre-interpolated.
        /// </summary>
        /// <remarks>
        /// <see langword="true"/> indicates that the client should perform additional interpolation on the response.
        /// </remarks>
        bool interp { get; }

    }

}