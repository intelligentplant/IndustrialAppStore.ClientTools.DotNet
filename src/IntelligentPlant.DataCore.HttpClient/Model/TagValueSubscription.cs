namespace IntelligentPlant.DataCore.Client.Model {

    /// <summary>
    /// Describes a subscription for receiving real-time value updates from Data Core.
    /// </summary>
    public class TagValueSubscription {

        /// <summary>
        /// Gets or sets the data source name.
        /// </summary>
        public string DataSourceName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the subscription group name.
        /// </summary>
        public string GroupName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the tag name.
        /// </summary>
        public string TagName { get; set; } = default!;

    }

}
