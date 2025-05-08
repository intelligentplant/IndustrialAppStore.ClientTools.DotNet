namespace IntelligentPlant.DataCore.Client.Model {
    /// <summary>
    /// Describes the health of a Data Core component.
    /// </summary>
    public class ComponentHealth {

        /// <summary>
        /// Gets a flag indicating if the driver is in good health.
        /// </summary>
        public bool IsHealthy { get; }

        /// <summary>
        /// Gets a collection of properties that contributed towards the overall health status.
        /// </summary>
        public IEnumerable<ComponentHealthProperty> Properties { get; }


        /// <summary>
        /// Creates a new <see cref="ComponentHealth"/> object.
        /// </summary>
        /// <param name="isHealthy">A flag indicating if the driver is in good health.</param>
        /// <param name="properties">A collection of properties that contributed towards the overall health status.</param>
        [Newtonsoft.Json.JsonConstructor]
        [System.Text.Json.Serialization.JsonConstructor]
        internal ComponentHealth(bool isHealthy, IEnumerable<ComponentHealthProperty> properties) {
            IsHealthy = isHealthy;
            Properties = properties?.ToArray() ?? System.Array.Empty<ComponentHealthProperty>();
        }

    }
}
