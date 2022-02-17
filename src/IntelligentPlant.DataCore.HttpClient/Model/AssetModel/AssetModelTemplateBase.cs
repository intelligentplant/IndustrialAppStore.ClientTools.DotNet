using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.DataCore.Client.Model.AssetModel {

    /// <summary>
    /// Base class for asset model item templates.
    /// </summary>
    public abstract class AssetModelTemplateBase {

        /// <summary>
        /// The unique identifier for the template.
        /// </summary>
        [Required]
        public string Id { get; set; } = default!;

        /// <summary>
        /// The template name.
        /// </summary>
        [Required]
        public string Name { get; set; } = default!;

        /// <summary>
        /// The template description.
        /// </summary>
        public string? Description { get; set; }

    }
}
