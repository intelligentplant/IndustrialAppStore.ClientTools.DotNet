using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.DataCore.Client.Model.Scripting {

    /// <summary>
    /// Describes a script tag definition.
    /// </summary>
    public class ScriptTagDefinition : ScriptTagSettings {

        /// <summary>
        /// Gets or sets the ID of the script tag.
        /// </summary>
        [Required]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the script tag metadata.
        /// </summary>
        public ScriptTagMetadata Metadata { get; set; }

    }
}
