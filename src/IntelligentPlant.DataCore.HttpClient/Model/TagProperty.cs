namespace IntelligentPlant.DataCore.Client.Model {
    /// <summary>
    /// Describes a tag property.
    /// </summary>
    public class TagProperty {

        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the category for the property.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the property description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the numerical order number for the property.
        /// </summary>
        /// <remarks>
        /// This property is intended to provide a hint to a user interface about the order that properties should be 
        /// displayed in inside their category.  Lower numbers have higher priority.
        /// </remarks>
        public int DisplayIndex { get; set; }

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public object Value { get; set; }

    }
}
