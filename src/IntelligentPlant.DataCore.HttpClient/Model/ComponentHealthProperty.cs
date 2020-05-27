using System;
using Newtonsoft.Json;

namespace IntelligentPlant.DataCore.Client.Model {
    /// <summary>
    /// Describes a property on a component that controbutes towards the overall health status of the component.
    /// </summary>
    public class ComponentHealthProperty {

        /// <summary>
        /// Gets the status property name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the healthy state of the property.
        /// </summary>
        public bool IsHealthy { get; }

        /// <summary>
        /// Gets the time stamp that the property was last updated at.
        /// </summary>
        public DateTime UtcTimeStamp { get; }

        /// <summary>
        /// Gets a message to accompany the health status property.
        /// </summary>
        public string Message { get; }


        /// <summary>
        /// Creates a new <see cref="ComponentHealthProperty"/> object.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <param name="isHealthy">A flag indicating if the property is in good health.</param>
        /// <param name="utcTimeStamp">The time stamp of the observation.</param>
        /// <param name="message">A message to accompany the property.</param>
        [JsonConstructor]
        internal ComponentHealthProperty(string name, bool isHealthy, DateTime utcTimeStamp, string message) {
            Name = name;
            IsHealthy = isHealthy;
            UtcTimeStamp = utcTimeStamp;
            Message = message;
        }

    }
}
