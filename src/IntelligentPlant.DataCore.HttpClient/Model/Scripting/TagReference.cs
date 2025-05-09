using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.DataCore.Client.Model.Scripting {

    /// <summary>
    /// Describes a tag referenced by a <see cref="ScriptTagDefinition"/>.
    /// </summary>
    public class TagReference {

        /// <summary>
        /// Gets or sets the data source name for the reference.
        /// </summary>
        [Required]
        [MaxLength(200)]
        [Newtonsoft.Json.JsonProperty("dsn")]
        [System.Text.Json.Serialization.JsonPropertyName("dsn")]
        public string DataSourceName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the tag name for the reference.
        /// </summary>
        [Required]
        [MaxLength(200)]
        [Newtonsoft.Json.JsonProperty("tag")]
        [System.Text.Json.Serialization.JsonPropertyName("tag")]
        public string TagName { get; set; } = default!;

        /// <summary>
        /// Controls which property of incoming <see cref="TagValue"/> objects for the referenced 
        /// tag get passed into the script.  By default, the numeric value is used.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("type")]
        [System.Text.Json.Serialization.JsonPropertyName("type")]
        public TagReferenceType Type { get; set; }

        /// <summary>
        /// When <see langword="true"/>, the script tag can only recalculate once a newer value has 
        /// been received for this tag reference.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("requiresNewerValue")]
        [System.Text.Json.Serialization.JsonPropertyName("requiresNewerValue")]
        public bool RequiresNewerValue { get; set; }

        /// <summary>
        /// When <see langword="false"/>, the script host will not pre-fetch any data for this tag 
        /// reference. This is useful if e.g. the script will call back into Data Core itself to 
        /// get data.
        /// </summary>
        [DefaultValue(true)]
        [Newtonsoft.Json.JsonProperty("preFetchData", DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Populate)]
        [System.Text.Json.Serialization.JsonPropertyName("preFetchData")]
        public bool PreFetchData { get; set; }


        /// <summary>
        /// Creates a new <see cref="TagReference"/> object.
        /// </summary>
        public TagReference() {
            PreFetchData = true;
        }

    }
}
