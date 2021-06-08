using System;

namespace IntelligentPlant.DataCore.Client.Model {
    /// <summary>
    /// Class describing a general-purpose log message.
    /// </summary>
    public class LogMessage {

        /// <summary>
        /// Gets or sets the UTC message timestamp.
        /// </summary>
        public DateTime UtcTimestamp { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

    }
}
