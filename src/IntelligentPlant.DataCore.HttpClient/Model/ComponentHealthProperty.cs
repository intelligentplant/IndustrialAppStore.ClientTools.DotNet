using System;

namespace IntelligentPlant.DataCore.Client.Model {
    /// <summary>
    /// Describes a property on a component that controbutes towards the overall health status of the component.
    /// </summary>
    public class ComponentHealthProperty {

        /// <summary>
        /// Gets the status property name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the healthy state of the property.
        /// </summary>
        public bool IsHealthy { get; set; }

        /// <summary>
        /// Gets the time stamp that the property was last updated at.
        /// </summary>
        public DateTime UtcTimeStamp { get; set; }

        /// <summary>
        /// Gets a message to accompany the health status property.
        /// </summary>
        public string Message { get; set; }

    }
}
