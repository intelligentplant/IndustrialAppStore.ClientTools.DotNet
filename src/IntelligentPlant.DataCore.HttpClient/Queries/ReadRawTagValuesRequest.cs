using System;
using System.Collections.Generic;
using System.Text;

namespace IntelligentPlant.DataCore.Client.Queries {

    /// <summary>
    /// Query for reading raw tag values.
    /// </summary>
    public class ReadRawTagValuesRequest : ReadTagValuesForTimeRangeRequest {

        /// <summary>
        /// The maximum number of samples to return per tag. The remote data sources may apply 
        /// more restrictive limits.
        /// </summary>
        public int PointCount { get; set; }

    }
}
