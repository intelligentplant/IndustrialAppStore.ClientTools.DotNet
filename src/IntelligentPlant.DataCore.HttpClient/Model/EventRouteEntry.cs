namespace IntelligentPlant.DataCore.Client.Model {
    /// <summary>
    /// Describes an entry in a route taken for an item such as an <see cref="EventMessage"/> from its source to its current location.
    /// </summary>
    /// <remarks>
    /// The <see cref="EventRouteEntry"/> class allows the journeys of items such as events that can be passed through a 
    /// chain of event sources and event sinks to be tracked.
    /// </remarks>
    public class EventRouteEntry : IEventRouteEntry {

        /// <summary>
        /// Gets or sets the UTC timestamp for the entry.
        /// </summary>
        public DateTime UtcTimestamp { get; set; }

        /// <summary>
        /// Gets or sets the name of the Data Core component that received the item.
        /// </summary>
        public string ComponentName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the host name for the server that received the item.
        /// </summary>
        public string HostName { get; set; } = default!;

    }
}
