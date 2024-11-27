namespace IntelligentPlant.DataCore.Client.Model.AssetModel {

    /// <summary>
    /// Describes an element template in an asset model.
    /// </summary>
    public class AssetModelElementTemplate : AssetModelTemplateBase {

        /// <summary>
        /// A flag specifying if the element template has property templates.
        /// </summary>
        public bool HasProperties { get; set; }

        /// <summary>
        /// The property templates associated with the element template.
        /// </summary>
        public IEnumerable<AssetModelPropertyTemplate>? Properties { get; set; }

    }
}
