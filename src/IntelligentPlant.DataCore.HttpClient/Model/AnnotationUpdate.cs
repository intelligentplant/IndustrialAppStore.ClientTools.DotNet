using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.DataCore.Client.Model {
    /// <summary>
    /// Describes an update to an annotation.
    /// </summary>
    public class AnnotationUpdate {

        /// <summary>
        /// Maximum length for <see cref="Description"/>.
        /// </summary>
        public const int MaximumDescriptionLength = 500;

        /// <summary>
        /// Maximum length for <see cref="Value"/>.
        /// </summary>
        public const int MaximumValueLength = 1000;

        /// <summary>
        /// Gets or sets the annotation identifier.  This also describes the tag that the annotation applies to, and the annotation time stamp.
        /// </summary>
        [Required]
        public AnnotationIdentifier Identifier { get; set; }

        /// <summary>
        /// Gets or sets a detailed description of the annotation.
        /// </summary>
        [MaxLength(MaximumDescriptionLength)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the annotation value.
        /// </summary>
        [Required]
        [MaxLength(MaximumValueLength)]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the name of the user who last modified the annotation.
        /// </summary>
        public string Modifier { get; set; }

    }
}
