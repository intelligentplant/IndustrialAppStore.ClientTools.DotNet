namespace IntelligentPlant.DataCore.Client.Model {

    /// <summary>
    /// Describes an entry in a route taken for an item such as an <see cref="IEventMessage"/> from its source to its current location.
    /// </summary>
    /// <remarks>
    /// The <see cref="IEventRouteEntry"/> interface allows the journeys of items such as events that can be passed through a 
    /// chain of event sources and event sinks to be tracked.
    /// </remarks>
    public interface IEventRouteEntry {

        /// <summary>
        /// Gets the UTC timestamp for the entry.
        /// </summary>
        DateTime UtcTimestamp { get; }

        /// <summary>
        /// Gets the name of the Data Core component that received the item.
        /// </summary>
        string ComponentName { get; }

        /// <summary>
        /// Gets the host name for the server that received the item.
        /// </summary>
        string HostName { get; }

    }

}