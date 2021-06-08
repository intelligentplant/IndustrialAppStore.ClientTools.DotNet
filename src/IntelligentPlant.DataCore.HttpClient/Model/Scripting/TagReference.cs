﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace IntelligentPlant.DataCore.Client.Model.Scripting {

    /// <summary>
    /// Describes a tag referenced by a <see cref="ScriptTagDefinition"/>.
    /// </summary>
    public class TagReference {

        /// <summary>
        /// Gets or sets the data source name for the reference.
        /// </summary>
        [Required]
        [JsonPropertyName("dsn")]
        [MaxLength(200)]
        public string DataSourceName { get; set; }

        /// <summary>
        /// Gets or sets the tag name for the reference.
        /// </summary>
        [Required]
        [JsonPropertyName("tag")]
        [MaxLength(200)]
        public string TagName { get; set; }

        /// <summary>
        /// Controls which property of incoming <see cref="TagValue"/> objects for the referenced 
        /// tag get passed into the script.  By default, the numeric value is used.
        /// </summary>
        [JsonPropertyName("type")]
        public TagReferenceType Type { get; set; }

        /// <summary>
        /// When <see langword="true"/>, the script tag can only recalculate once a newer value has 
        /// been received for this tag reference.
        /// </summary>
        [JsonPropertyName("requiresNewerValue")]
        public bool RequiresNewerValue { get; set; }

        /// <summary>
        /// When <see langword="false"/>, the script host will not pre-fetch any data for this tag 
        /// reference. This is useful if e.g. the script will call back into Data Core itself to 
        /// get data.
        /// </summary>
        [JsonPropertyName("preFetchData")]
        [DefaultValue(true)]
        public bool PreFetchData { get; set; } = true;

    }
}
