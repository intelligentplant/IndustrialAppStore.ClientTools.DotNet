using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.DataCore.Client.Model.Scripting {
    /// <summary>
    /// Describes a script engine registered with Data Core.
    /// </summary>
    public class ScriptEngine {

        /// <summary>
        /// Gets or sets the script engine ID.
        /// </summary>
        [Required]
        public string Id { get; set; } = default!;

        /// <summary>
        /// Gets or sets the display name for the script engine.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = default!;

        /// <summary>
        /// Gets or sets the description for the script engine.
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the name of the scripting language used by the engine.
        /// </summary>
        [MaxLength(50)]
        public string? Language { get; set; }

        /// <summary>
        /// Gets or sets a flag specifying if the script engine supports templating.
        /// </summary>
        public bool SupportsTemplates { get; set; }

        /// <summary>
        /// Gets or sets a flag specifying if the script engine supports ad hoc scripts. When <see langword="false"/>, 
        /// this indicates that scripts can only be created via templates.
        /// </summary>
        public bool SupportsAdHocScripts { get; set; }

    }
}
