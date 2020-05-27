using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.DataCore.Client.Model.Scripting.Templates {

    /// <summary>
    /// Describes a script template.
    /// </summary>
    public class ScriptTemplate {

        /// <summary>
        /// Gets or sets the script language for the template.
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the template category.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the template name.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the template description.
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the template version.
        /// </summary>
        [Required]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the template author.
        /// </summary>
        public Person Author { get; set; }

        /// <summary>
        /// Gets or sets the template contributors.
        /// </summary>
        public IEnumerable<Person> Contributors { get; set; }

    }
}
