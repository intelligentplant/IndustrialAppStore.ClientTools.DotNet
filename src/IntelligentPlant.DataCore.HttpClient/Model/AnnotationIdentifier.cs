using System;
using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.DataCore.Client.Model {
    /// <summary>
    /// Describes an identifier for an annotation.
    /// </summary>
    public class AnnotationIdentifier : IEquatable<AnnotationIdentifier> {

        /// <summary>
        /// Gets or sets a qualifying identifier for the annotation, to distinguish it from other annotations with the same tag name and time stamp.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the tag that the annotation applies to.
        /// </summary>
        [Required]
        public string TagName { get; set; }

        /// <summary>
        /// Gets or sets the UTC timestamp for the annotation.
        /// </summary>
        [Required]
        public DateTime UtcAnnotationTime { get; set; }


        /// <summary>
        /// Gets the hash code for the object.
        /// </summary>
        /// <returns>
        /// The hash code.
        /// </returns>
        public override int GetHashCode() {
            return (Id + TagName + UtcAnnotationTime.ToString("O")).GetHashCode();
        }


        /// <summary>
        /// Tests if another <see cref="AnnotationIdentifier"/> is equivalent to the current instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        /// <returns>
        /// <see langword="true"/> if the two instances are equivalent, otherwise <see langword="false"/>.
        /// </returns>
        public bool Equals(AnnotationIdentifier other) {
            if (other == null) {
                return false;
            }

            return String.Equals(Id, other.Id, StringComparison.OrdinalIgnoreCase) &&
                   String.Equals(TagName, other.TagName, StringComparison.OrdinalIgnoreCase) &&
                   UtcAnnotationTime.Equals(other.UtcAnnotationTime);
        }


        /// <summary>
        /// Tests if another object is equivalent to the current object.
        /// </summary>
        /// <param name="obj">The other instance.</param>
        /// <returns>
        /// <see langword="true"/> if the two instances are equivalent, otherwise <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj) {
            return Equals(obj as AnnotationIdentifier);
        }

    }
}
