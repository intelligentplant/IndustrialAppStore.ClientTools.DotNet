using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.DataCore.Client.Model {

    /// <summary>
    /// Describes a Data Core event stream message.
    /// </summary>
    public class EventMessage {

        /// <summary>
        /// Gets or sets the ID for the event object.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the ID or name of the event source that generated the event.
        /// </summary>
        public string SourceSystem { get; set; }

        /// <summary>
        /// Gets or sets the source identifier for the event.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the UTC event time.
        /// </summary>
        public DateTime UtcEventTime { get; set; }

        /// <summary>
        /// Gets or sets the event category.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the event name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a sequence number for the message.
        /// </summary>
        /// <remarks>
        /// This is an ordinal that can be used in conjunction with <see cref="UtcEventTime"/> to determine the order 
        /// that event messages were receive.
        /// </remarks>
        public long Seq { get; set; }

        /// <summary>
        /// Gets or sets the event message text.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the event properties.
        /// </summary>
        [Required]
        public ICollection<EventProperty> Properties { get; set; } = new List<EventProperty>();

        /// <summary>
        /// Gets or sets the raw message from the event source.
        /// </summary>
        /// <remarks>
        /// For event sources such as control systems, this field should be populated with the message that was received from the source.
        /// </remarks>
        public string RawMessage { get; set; }

        /// <summary>
        /// Gets or sets the route that the event has taken to get to its current location.
        /// </summary>
        [Required]
        public ICollection<EventRouteEntry> Route { get; set; } = new List<EventRouteEntry>();

    }

}
