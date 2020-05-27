using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.DataCore.Client.Model.AssetModel {

    /// <summary>
    /// Base class for items in an asset model.
    /// </summary>
    public abstract class AssetModelItemBase {

        /// <summary>
        /// The unique identifier for the model item.
        /// </summary>
        [Required]
        public string Id { get; set; }

        /// <summary>
        /// The name of the item.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// The description for the item.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The template for the item. Can be <see langword="null"/> if the item does not use a 
        /// template.
        /// </summary>
        public string TemplateId { get; set; }

        /// <summary>
        /// The full address of the item.
        /// </summary>
        [Required]
        public string Address { get; set; }

    }
}
