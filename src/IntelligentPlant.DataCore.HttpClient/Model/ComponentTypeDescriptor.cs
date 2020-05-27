namespace IntelligentPlant.DataCore.Client.Model {

    /// <summary>
    /// Describes a component type.
    /// </summary>
    public class ComponentTypeDescriptor {
    
        /// <summary>
        /// The type name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The display name or description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The type version.
        /// </summary>
        public string Version { get; set; }

    }
}
