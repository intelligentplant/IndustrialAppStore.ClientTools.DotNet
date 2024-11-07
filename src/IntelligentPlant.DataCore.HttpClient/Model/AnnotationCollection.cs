namespace IntelligentPlant.DataCore.Client.Model {
    /// <summary>
    /// Describes a collection of annotations associated with a tag.
    /// </summary>
    public class AnnotationCollection {

        /// <summary>
        /// Gets or sets the tag name.
        /// </summary>
        public string TagName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the annotations.
        /// </summary>
        public IEnumerable<Annotation> Annotations { get; set; } = default!;

    }
}
