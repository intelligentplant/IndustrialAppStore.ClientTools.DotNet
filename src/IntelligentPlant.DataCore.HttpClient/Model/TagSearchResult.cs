using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.DataCore.Client.Model {

    /// <summary>
    /// Describes a tag search result.
    /// </summary>
    public class TagSearchResult {

        /// <summary>
        /// Gets or sets the tag ID.
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Gets or sets the tag name.
        /// </summary>
        [Required]
        public string Name { get; set; } = default!;

        /// <summary>
        /// Gets or sets the tag description.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the tag's unit of measure.
        /// </summary>
        public string? UnitOfMeasure { get; set; }

        /// <summary>
        /// Tag properties
        /// </summary>
        private IDictionary<string, TagProperty> _properties = new Dictionary<string, TagProperty>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets or sets the collection of properties associated with the tag.
        /// </summary>
        public IDictionary<string, TagProperty> Properties {
            get { return _properties; }
            set { _properties = value ?? new Dictionary<string, TagProperty>(StringComparer.OrdinalIgnoreCase); }
        }

        /// <summary>
        /// Digital states
        /// </summary>
        private ICollection<TagDigitalState> _digitalStates = new List<TagDigitalState>();

        /// <summary>
        /// Gets or sets the digital or enumerated states associated with the tag,
        /// </summary>
        public ICollection<TagDigitalState> DigitalStates {
            get { return _digitalStates; }
            set { _digitalStates = value ?? new List<TagDigitalState>(); }

        }

        /// <summary>
        /// The labels for the tag.
        /// </summary>
        public ICollection<string>? Labels { get; set; }


        /// <summary>
        /// Creates a new <see cref="TagSearchResult"/> object.
        /// </summary>
        public TagSearchResult() : this(null) { }


        /// <summary>
        /// Creates a new <see cref="TagSearchResult"/> object that is a copy of an existing object.
        /// </summary>
        /// <param name="other">The existing tag definition to copy.</param>
        public TagSearchResult(TagSearchResult? other) {
            if (other == null) {
                return;
            }

            Name = other.Name;
            Description = other.Description;
            UnitOfMeasure = other.UnitOfMeasure;
            Properties = other.Properties;
            DigitalStates = other.DigitalStates;
            Labels = other.Labels;
        }


        /// <summary>
        /// Gets the value of the specified tag property.
        /// </summary>
        /// <typeparam name="T">The property value type.</typeparam>
        /// <param name="name">The property name.</param>
        /// <param name="defaultValue">The value to return if the property is undefined, or if it cannot be cast to <typeparamref name="T"/>.</param>
        /// <returns>
        /// The property value, or <paramref name="defaultValue"/> if the property is undefined, or if it cannot be cast to <typeparamref name="T"/>.
        /// </returns>
        public T? GetTagPropertyValue<T>(string name, T defaultValue) {
            if (name == null) {
                return defaultValue;
            }

            if (!Properties.TryGetValue(name, out var prop)) {
                return defaultValue;
            }

            return prop.GetValueOrDefault(defaultValue);
        }


        /// <summary>
        /// Gets the value of the specified tag property.
        /// </summary>
        /// <typeparam name="T">The property value type.</typeparam>
        /// <param name="name">The property name.</param>
        /// <returns>
        /// The property value, or the default value of <typeparamref name="T"/> if the property is undefined or cannot be cast to <typeparamref name="T"/>.
        /// </returns>
        public T? GetTagPropertyValue<T>(string name) {
            return GetTagPropertyValue(name, default(T));
        }

    }

}
