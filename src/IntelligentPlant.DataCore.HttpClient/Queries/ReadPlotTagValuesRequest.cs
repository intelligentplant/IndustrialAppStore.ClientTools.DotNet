using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IntelligentPlant.DataCore.Client.Queries {

    /// <summary>
    /// Query for reading visualization-friendly historical tag values.
    /// </summary>
    public class ReadPlotTagValuesRequest : ReadTagValuesForTimeRangeRequest {

        /// <summary>
        /// The number of intervals to use in the query. Plot queries work by dividing the time 
        /// range into an equal number of intervals, and then selecting significant raw values 
        /// from each interval. Therefore, the higher the interval count, the more values will 
        /// be returned. The implementation of plot queries is left to the historian vendor, 
        /// but a good rule of thumb is that up to 4 samples might be returned in each interval 
        /// (typically the earliest, latest, minimum and maximum values).
        /// </summary>
        [Required]
        [Range(1, int.MaxValue)]
        public int Intervals { get; set; }

    }
}
