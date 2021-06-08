using System;
using System.Collections.Generic;

namespace IntelligentPlant.DataCore.Client.Model {
    /// <summary>
    /// Describes the health of a Data Core component.
    /// </summary>
    public class ComponentHealth {

        /// <summary>
        /// Gets a flag indicating if the driver is in good health.
        /// </summary>
        public bool IsHealthy { get; set; }

        /// <summary>
        /// Gets a collection of properties that contributed towards the overall health status.
        /// </summary>
        public IEnumerable<ComponentHealthProperty> Properties { get; set; } = Array.Empty<ComponentHealthProperty>(); 

    }
}
