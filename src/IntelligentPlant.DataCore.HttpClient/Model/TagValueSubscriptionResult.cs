using System;
using System.Collections.Generic;

namespace IntelligentPlant.DataCore.Client.Model {
    /// <summary>
    /// Describes the result of a subscription request for real-time tag values.
    /// </summary>
    public class TagValueSubscriptionResult {

        /// <summary>
        /// Gets or sets the data source name.
        /// </summary>
        public string DataSourceName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the data source's push notification group that the tags have been assigned to.
        /// </summary>
        public string GroupName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the registration results, indexed by tag name.
        /// </summary>
        public IDictionary<string, TagValueSubscriptionResultItem> Result { get; set; } = default!;

        /// <summary>
        /// Gets or sets messages associated with the registration.
        /// </summary>
        public IList<TagValueSubscriptionResultMessage> Messages { get; set; } = default!;

        /// <summary>
        /// Gets or sets the time taken to perform the registration.
        /// </summary>
        public TimeSpan RegistrationTime { get; set; }

    }
}
