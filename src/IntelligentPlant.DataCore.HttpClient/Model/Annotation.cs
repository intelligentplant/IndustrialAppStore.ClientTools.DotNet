using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.DataCore.Client.Model {
    /// <summary>
    /// Describes an annotation on a tag.
    /// </summary>
    public class Annotation : AnnotationUpdate, IEquatable<Annotation> {

        /// <summary>
        /// Maximum length for <see cref="ApplicationName"/>.
        /// </summary>
        public const int MaximumApplicationNameLength = 50;

        /// <summary>
        /// Gets or sets the application name for the annotation.
        /// </summary>
        [MaxLength(MaximumApplicationNameLength)]
        public string? ApplicationName { get; set; }


        /// <summary>
        /// Gets or sets the UTC creation time for the annotation.
        /// </summary>
        public DateTime? UtcCreationTime { get; set; }

        /// <summary>
        /// Gets or sets the name of the user who created the annotation.
        /// </summary>
        public string? Creator { get; set; }

        /// <summary>
        /// Gets or sets the UTC last modified time for the annotation.
        /// </summary>
        public DateTime? UtcModifiedTime { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if the annotation is read-only or not.
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Gets or sets a URL that can be followed to obtain more information about an annotation. This 
        /// is intended to allow annotations that are coming from secondary sources (e.g. Facit or Alarm 
        /// Analysis) to provide links where e.g. the workflow associated with the annotation can be 
        /// inspected.
        /// </summary>
        public Uri? MoreInfo { get; set; }


        /// <summary>
        /// Gets the hash code for the object.
        /// </summary>
        /// <returns>
        /// The hash code.
        /// </returns>
        public override int GetHashCode() {
            return Identifier == null ? base.GetHashCode() : Identifier.GetHashCode();
        }


        /// <summary>
        /// Tests if another <see cref="Annotation"/> is equivalent to the current instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        /// <returns>
        /// <see langword="true"/> if the two instances are equivalent, otherwise <see langword="false"/>.
        /// </returns>
        public bool Equals(Annotation? other) {
            if (other == null) {
                return false;
            }
            if (Identifier == null && other.Identifier == null) {
                return ReferenceEquals(this, other);
            }
            if (Identifier == null || other.Identifier == null) {
                return false;
            }

            return Identifier.Equals(other.Identifier);
        }


        /// <summary>
        /// Tests if another object is equivalent to the current object.
        /// </summary>
        /// <param name="obj">The other instance.</param>
        /// <returns>
        /// <see langword="true"/> if the two instances are equivalent, otherwise <see langword="false"/>.
        /// </returns>
        public override bool Equals(object? obj) {
            return obj is Annotation annotation && Equals(annotation);
        }

    }
}
