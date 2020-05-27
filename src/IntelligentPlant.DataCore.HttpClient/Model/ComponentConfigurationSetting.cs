
namespace IntelligentPlant.DataCore.Client.Model {

    /// <summary>
    /// Describes a component configuration setting.
    /// </summary>
    public class ComponentConfigurationSetting {

        /// <summary>
        /// Gets or sets the setting category.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets the setting display name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the setting description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the .NET CLR type name for the setting.
        /// </summary>
        /// <remarks>
        /// The type name should be assembly qualified.
        /// </remarks>
        public string TypeName { get; set; }

        /// <summary>
        /// Gets or sets the current setting value.
        /// </summary>
        public string Value { get; set; }

    }
}
