using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IntelligentPlant.DataCore.Client.Queries {

    /// <summary>
    /// A query to retrieve historical tag values at specific sample times.
    /// </summary>
    public class ReadTagValuesAtTimesRequest : ReadTagValuesRequest {

        /// <summary>
        /// The sample times to retrieve tag values at.
        /// </summary>
        [Required]
        [MinLength(1)]
        public DateTime[] UtcSampleTimes { get; set; }

    }
}
