using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.DataCore.Client.Model.AssetModel {

    /// <summary>
    /// Describes a property on an <see cref="AssetModelElement"/>.
    /// </summary>
    public class AssetModelProperty : AssetModelItemBase {

        /// <summary>
        /// The ID of the <see cref="AssetModelElement"/> that the property belongs to.
        /// </summary>
        [Required]
        public string ElementId { get; set; } = default!;

        /// <summary>
        /// Gets or sets the unit of measure for the property.
        /// </summary>
        public string? UnitOfMeasure { get; set; }

        /// <summary>
        /// The value of the property.
        /// </summary>
        public object? Value { get; set; }

    }
}
