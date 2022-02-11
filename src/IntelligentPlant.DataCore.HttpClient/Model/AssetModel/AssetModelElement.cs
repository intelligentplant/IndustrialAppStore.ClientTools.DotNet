using System.Collections.Generic;

namespace IntelligentPlant.DataCore.Client.Model.AssetModel {

    /// <summary>
    /// Describes an element (node) in an asset model hierarchy.
    /// </summary>
    public class AssetModelElement : AssetModelItemBase {

        /// <summary>
        /// The parent ID of the element. Can be <see langword="null"/> if this is a top-level 
        /// element in the hierarchy.
        /// </summary>
        public string? ParentId { get; set; }

        /// <summary>
        /// A flag specifying if the element has children.
        /// </summary>
        public bool HasChildren { get; set; }

        /// <summary>
        /// The child elements for this element.
        /// </summary>
        public IEnumerable<AssetModelElement>? Children { get; set; }

        /// <summary>
        /// A flag specifying if the element has properties.
        /// </summary>
        public bool HasProperties { get; set; }

        /// <summary>
        /// The properties for the element.
        /// </summary>
        public IEnumerable<AssetModelProperty>? Properties { get; set; }

    }
}
