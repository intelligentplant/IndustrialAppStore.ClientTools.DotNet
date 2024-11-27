using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.DataCore.Client.Model {
    /// <summary>
    /// Describes a Data Core component.
    /// </summary>
    public class ComponentInfo {

        /// <summary>
        /// Gets or sets the component name.
        /// </summary>
        [Required]
        public ComponentName Name { get; set; } = default!;

        /// <summary>
        /// Gets or sets the component description.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the component type name.
        /// </summary>
        [Required]
        public string TypeName { get; set; } = default!;

        /// <summary>
        /// Gets the current component status.
        /// </summary>
        public ComponentStatus Status { get; set; } = default!;

        /// <summary>
        /// Gets or sets the properties for the component.
        /// </summary>
        public IDictionary<string, string>? Properties { get; set; }

    }
}
