using System.Collections.Generic;

namespace IntelligentPlant.DataCore.Client.Model {
    /// <summary>
    /// POST-ed annotation data request.
    /// </summary>
    public class GetAnnotationsRequest {

        /// <summary>
        /// Gets or sets the data source name.
        /// </summary>
        public string Dsn { get; set; }

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        public IEnumerable<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets the query start time.
        /// </summary>
        public string Start { get; set; }

        /// <summary>
        /// Gets or sets the query start time.
        /// </summary>
        public string End { get; set; }

    }
}
