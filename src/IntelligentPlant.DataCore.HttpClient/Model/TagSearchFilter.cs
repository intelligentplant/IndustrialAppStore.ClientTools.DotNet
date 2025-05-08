using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.DataCore.Client.Model {
    /// <summary>
    /// Describes a tag search filter.
    /// </summary>
    public class TagSearchFilter : IValidatableObject {

        /// <summary>
        /// Default page size to use.
        /// </summary>
        private const int DefaultPageSize = 50;

        /// <summary>
        /// Maximum page size to use.
        /// </summary>
        public const int MaximumPageSize = 100;

        /// <summary>
        /// Gets or sets the tag name filter to use.
        /// </summary>
        [MaxLength(255)]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the tag description filter to use.
        /// </summary>
        [MaxLength(100)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the tag unit filter to use.
        /// </summary>
        [MaxLength(100)]
        public string? Unit { get; set; }

        /// <summary>
        /// The label filter to use.
        /// </summary>
        [MaxLength(100)]
        public string? Label { get; set; }

        /// <summary>
        /// Gets or sets the additional tag property filters to use.
        /// </summary>
        public IDictionary<string, string>? Other { get; set; }

        /// <summary>
        /// Gets or sets the page size for the results.
        /// </summary>
        [Range(1, MaximumPageSize)]
        public int PageSize { get; set; } = DefaultPageSize;

        /// <summary>
        /// Gets or sets the page number of the matching results that should be returned.
        /// </summary>
        /// <remarks>
        /// Setting <see cref="Page"/> to be less than 1 will automatically set the value to 1 instead.
        /// </remarks>
        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        /// <summary>
        /// Gets or sets the names of the driver-specific properties that should be included in each search result.
        /// </summary>
        /// <remarks>
        /// 
        /// <para>
        /// If <see cref="Properties"/> is <see langword="null"/>, all available properties will be returned.
        /// </para>
        /// 
        /// <para>
        /// If <see cref="Properties"/> is empty, no properties will be returned.
        /// </para>
        /// 
        /// </remarks>
        public IEnumerable<string>? Properties { get; set; }


        /// <summary>
        /// Creates a new <see cref="TagSearchFilter"/> object using the specified name filter.
        /// </summary>
        /// <param name="name">The tag name filter.</param>
        public TagSearchFilter(string? name) : this(name, null, null) { }


        /// <summary>
        /// Creates a new <see cref="TagSearchFilter"/> object using the specified name, description and unit filters.
        /// </summary>
        /// <param name="name">The tag name filter.</param>
        /// <param name="description">The description filter.</param>
        public TagSearchFilter(string? name, string? description) : this(name, description, null) { }


        /// <summary>
        /// Creates a new <see cref="TagSearchFilter"/> object using the specified name, description and unit filters.
        /// </summary>
        /// <param name="name">The tag name filter.</param>
        /// <param name="description">The description filter.</param>
        /// <param name="unit">The tag unit filter.</param>
        [Newtonsoft.Json.JsonConstructor]
        [System.Text.Json.Serialization.JsonConstructor]
        public TagSearchFilter(string? name, string? description, string? unit) {
            Name = name;
            Description = description;
            Unit = unit;

            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(description) && string.IsNullOrWhiteSpace(unit)) {
                Name = "*";
            }
        }


        /// <summary>
        /// Performs custom validation on the object.
        /// </summary>
        /// <param name="validationContext">The validation context.</param>
        /// <returns>
        /// A collection of validation errors.
        /// </returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            const int maximumAdditionalFilterCount = 5;
            const int minimumAdditionalFilterLength = 1;
            const int maximumAdditionalFilterNameLength = 100;
            const int maximumAdditionalFilterValueLength = 100;

            if (Other != null) {
                if (Other.Count > maximumAdditionalFilterCount) {
                    yield return new ValidationResult($"Maximum number of additional filters is {maximumAdditionalFilterCount}.", new[] { nameof(Other) });
                }

                foreach (var item in Other) {
                    if (item.Key.Length < minimumAdditionalFilterLength) {
                        yield return new ValidationResult($"Minimum length of an additional filter name is {minimumAdditionalFilterLength}.", new[] { nameof(Other) });
                    }
                    if (item.Key.Length > maximumAdditionalFilterNameLength) {
                        yield return new ValidationResult($"Maximum length of an additional filter name is {maximumAdditionalFilterNameLength}.", new[] { nameof(Other) });
                    }

                    if (item.Value == null || item.Value.Length < minimumAdditionalFilterLength) {
                        yield return new ValidationResult($"Minimum length of an additional filter value is {minimumAdditionalFilterLength}.", new[] { nameof(Other) });
                    }
                    if (item.Value?.Length > maximumAdditionalFilterValueLength) {
                        yield return new ValidationResult($"Maximum length of an additional filter value is {maximumAdditionalFilterValueLength}.", new[] { nameof(Other) });
                    }
                }
            }
        }
    }
}
