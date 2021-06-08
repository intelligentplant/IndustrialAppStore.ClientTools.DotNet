namespace IntelligentPlant.DataCore.Client.Model {
    /// <summary>
    /// Describes a property associated with an event message.
    /// </summary>
    public class EventProperty {

        /// <summary>
        /// Gets or sets the event name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the event value.
        /// </summary>
        public object Value { get; set; }

    }
}
