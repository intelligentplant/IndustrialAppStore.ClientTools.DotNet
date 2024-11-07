using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.DataCore.Client.Model.Scripting {

    /// <summary>
    /// Describes the settings for a script tag.
    /// </summary>
    public class ScriptTagSettings : ScriptTagSettingsBase {

        /// <summary>
        /// Gets or sets the tag name.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = default!;

        /// <summary>
        /// Gets or sets the tag description.
        /// </summary>
        [MaxLength(200)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets a flag that indicates if the script tag is enabled or disabled. Disabled script 
        /// tags cannot be evaluated.
        /// </summary>
        [DefaultValue(true)]
        [Newtonsoft.Json.JsonProperty(DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Populate)]
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the tag units.
        /// </summary>
        [MaxLength(50)]
        public string? Units { get; set; }

        /// <summary>
        /// Gets or sets the labels associated with the script tag.
        /// </summary>
        [MaxLength(20)]
        public string[]? Labels { get; set; }

        /// <summary>
        /// Gets or sets the archive settings for the tag. Archiving must be enabled if you want to 
        /// perform historical queries on the script tag.
        /// </summary>
        public ScriptTagArchiveSettings? ArchiveSettings { get; set; }

    }


    /// <summary>
    /// Describes a request to create a script tag.
    /// </summary>
    public class CreateScriptTagSettings : ScriptTagSettings {

        /// <summary>
        /// Gets or sets the ID of the script engine that will be used to run the tag.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string ScriptEngineId { get; set; } = default!;

        /// <summary>
        /// Gets or sets the name of the application that is creating the tag.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string ApplicationName { get; set; } = default!;

        /// <summary>
        /// The initial start time for the script tag calculation, used on the first evaluation of 
        /// the script only.
        /// </summary>
        /// <remarks>
        ///   If an initial start time is not specified, an appropriate start time will be 
        ///   inferred when the script tag is first evaluated.
        /// </remarks>
        public DateTime? InitialSampleTime { get; set; }

    }
}
