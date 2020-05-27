using System;
using System.Collections.Generic;

namespace IntelligentPlant.DataCore.Client.Model {

    /// <summary>
    /// Interface that describes a Data Core event message.
    /// </summary>
    public interface IEventMessage {

        /// <summary>
        /// Gets or sets the ID for the event object.
        /// </summary>
        string ID { get; }

        /// <summary>
        /// Gets or sets the ID or name of the event source that generated the event.
        /// </summary>
        string SourceSystem { get; }

        /// <summary>
        /// Gets or sets the source identifier for the event.
        /// </summary>
        string Source { get; }

        /// <summary>
        /// Gets or sets the UTC event time.
        /// </summary>
        DateTime UtcEventTime { get; }

        /// <summary>
        /// Gets or sets the event category.
        /// </summary>
        string Category { get; }

        /// <summary>
        /// Gets or sets the event name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets or sets a sequence number for the message.
        /// </summary>
        /// <remarks>
        /// This is an ordinal that can be used in conjunction with <see cref="UtcEventTime"/> to determine the order 
        /// that event messages were receive.
        /// </remarks>
        long Seq { get; }

        /// <summary>
        /// Gets or sets the event message text.
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Gets or sets the event properties.
        /// </summary>
        ICollection<EventProperty> Properties { get; }

        /// <summary>
        /// Gets or sets the raw message from the event source.
        /// </summary>
        /// <remarks>
        /// For event sources such as control systems, this field should be populated with the message that was received from the source.
        /// </remarks>
        string RawMessage { get; }

        /// <summary>
        /// Gets or sets the route that the event has taken to get to its current location.
        /// </summary>
        ICollection<EventRouteEntry> Route { get; }

    }

}