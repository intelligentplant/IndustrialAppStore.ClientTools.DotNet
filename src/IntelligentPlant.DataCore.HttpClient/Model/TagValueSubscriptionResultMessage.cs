namespace IntelligentPlant.DataCore.Client.Model {
    /// <summary>
    /// Describes a message associated with a <see cref="TagValueSubscriptionResult"/> object.
    /// </summary>
    public class TagValueSubscriptionResultMessage {

        /// <summary>
        /// Gets or sets the message category.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public object Message { get; set; }

    }
}
