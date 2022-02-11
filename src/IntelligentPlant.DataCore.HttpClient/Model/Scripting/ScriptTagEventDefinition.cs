using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.DataCore.Client.Model.Scripting {
    /// <summary>
    /// Describes an event definition that can be triggered or reset by a script tag.
    /// </summary>
    public class ScriptTagEventDefinition {

        /// <summary>
        /// Gets or sets the priority for the event.
        /// </summary>
        [DefaultValue(ScriptTagEventPriority.Unknown)]
        [Newtonsoft.Json.JsonProperty(DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Populate)]
        public ScriptTagEventPriority Priority { get; set; }

        /// <summary>
        /// Gets or sets the namespace for the event. If not specified, the name of the script tag will be used.
        /// </summary>
        [MaxLength(200)]
        public string? Namespace { get; set; }

        /// <summary>
        /// Gets or sets the category for the event.
        /// </summary>
        [MaxLength(100)]
        public string? Category { get; set; }

        /// <summary>
        /// Gets or sets the description for the event.
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets a flag specifying if the event must reset before it can trigger again.
        /// </summary>
        [DefaultValue(true)]
        [Newtonsoft.Json.JsonProperty(DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Populate)]
        public bool RequiresReset { get; set; }

        /// <summary>
        /// Gets or sets a URL that can be visited to find out more information about an event.
        /// </summary>
        [MaxLength(500)]
        public string? MoreInfoUrl { get; set; }

        /// <summary>
        /// Gets or sets the caption to use for "more info" links.
        /// </summary>
        [MaxLength(50)]
        public string? MoreInfoCaption { get; set; }

    }


    /// <summary>
    /// Describes the priority/severity to associate with an event that is triggered or reset by a 
    /// script tag.
    /// </summary>
    public enum ScriptTagEventPriority {

        /// <summary>
        /// Unknown priority.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Low priority.
        /// </summary>
        Low = 1,

        /// <summary>
        /// Medium priority.
        /// </summary>
        Medium = 2,

        /// <summary>
        /// High priority.
        /// </summary>
        High = 3,

        /// <summary>
        /// Critical priority.
        /// </summary>
        Critical = 4

    }

}
