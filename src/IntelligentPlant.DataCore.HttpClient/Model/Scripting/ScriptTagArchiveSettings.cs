using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.DataCore.Client.Model.Scripting {

    /// <summary>
    /// Settings that specify how script tag values should be archived.
    /// </summary>
    public class ScriptTagArchiveSettings: IValidatableObject {

        /// <summary>
        /// Specifies if snapshot values for the script tag will automatically be archived.
        /// </summary>
        public bool IsArchivingEnabled { get; set; }

        /// <summary>
        /// Specifies the destination data source to write values to when <see cref="IsArchivingEnabled"/> 
        /// is <see langword="true"/>.
        /// </summary>
        public string? ArchivingDataSourceName { get; set; }

        /// <summary>
        /// Specifies the destination tag name to write values to when <see cref="IsArchivingEnabled"/> is 
        /// <see langword="true"/>. A <see langword="null"/> or white space value means that a destination 
        /// tag name will be inferred.
        /// </summary>
        public string? ArchivingTagName { get; set; }


        /// <summary>
        /// Validates the object.
        /// </summary>
        /// <param name="validationContext">
        ///   The validation context.
        /// </param>
        /// <returns>
        ///   The validation errors.
        /// </returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (IsArchivingEnabled) {
                if (String.IsNullOrWhiteSpace(ArchivingDataSourceName)) {
                    yield return new ValidationResult(Resources.Error_InvalidDataSourceName, new[] { nameof(ArchivingDataSourceName) });
                }
            }
        }

    }
}
