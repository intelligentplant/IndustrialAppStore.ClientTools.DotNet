using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace IntelligentPlant.DataCore.Client.Queries {

    /// <summary>
    /// Base request class when reading tag values or annotations.
    /// </summary>
    public abstract class ReadTagValuesRequest : IValidatableObject {

        /// <summary>
        /// The tags to query, indexed by data source name.
        /// </summary>
        [Required]
        public IDictionary<string, string[]> Tags { get; set; }

        /// <summary>
        /// Additional custom query properties.
        /// </summary>
        public IDictionary<string, string> QueryProperties { get; set; }

        /// <summary>
        /// Validates the object.
        /// </summary>
        /// <param name="validationContext">
        ///   The validation context.
        /// </param>
        /// <returns>
        ///   The validation errors.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="validationContext"/> is <see langword="null"/>.
        /// </exception>
        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext) {
            if (validationContext == null) {
                throw new ArgumentNullException(nameof(validationContext));
            }
            return ValidateInternal(validationContext);
        }


        /// <summary>
        /// Validates the object.
        /// </summary>
        /// <param name="validationContext">
        ///   The validation context.
        /// </param>
        /// <returns>
        ///   The validation errors.
        /// </returns>
        private IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext) {
            if (Tags != null) {
                foreach (var item in Tags) {
                    if (item.Value == null || item.Value.Any(x => x == null)) {
                        yield return new ValidationResult(Resources.Error_TagMapOrListCannotContainNullItems, new[] { nameof(Tags) });
                    }
                }
            }

            foreach (var item in Validate(validationContext)) {
                yield return item;
            }
        }


        /// <summary>
        /// Validates the object. Override this method in an ineriting class to add additional 
        /// custom validation.
        /// </summary>
        /// <param name="validationContext">
        ///   The validation context.
        /// </param>
        /// <returns>
        ///   The validation errors.
        /// </returns>
        protected virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            return Array.Empty<ValidationResult>();
        }

    }
}
