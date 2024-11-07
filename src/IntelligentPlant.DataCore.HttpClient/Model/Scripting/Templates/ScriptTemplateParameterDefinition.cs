using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.DataCore.Client.Model.Scripting.Templates {
    /// <summary>
    /// Describes a parameter that can be configured in a script template.
    /// </summary>
    public class ScriptTemplateParameterDefinition {

        /// <summary>
        /// The maximum length of a parameter name.
        /// </summary>
        public const int MaximumNameLength = 100;

        /// <summary>
        /// Gets or sets the name of the parameter.
        /// </summary>
        [Required]
        [MaxLength(MaximumNameLength)]
        public string Name { get; set; } = default!;

        /// <summary>
        /// Gets or sets the friendly (display) name for the parameter.
        /// </summary>
        [MaxLength(200)]
        public string? FriendlyName { get; set; }

        /// <summary>
        /// Gets or sets the description for the parameter.
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the help link for the parameter.
        /// </summary>
        [MaxLength(200)]
        public Uri? HelpLink { get; set; }

        /// <summary>
        /// Gets or sets the type of the parameter. This can be used to infer the type of editor that 
        /// a user is shown when creating a new item using the template.
        /// </summary>
        public SimpleType Type { get; set; }

        /// <summary>
        /// Gets or sets the default value for the parameter.
        /// </summary>
        public object? DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the validators for the parameter.
        /// </summary>
        public IEnumerable<IDictionary<string, object>>? Validators { get; set; }

    }
}
