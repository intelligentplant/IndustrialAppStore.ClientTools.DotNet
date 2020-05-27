namespace IntelligentPlant.DataCore.Client.Model.AssetModel {

    /// <summary>
    /// Describes a property template in an asset model.
    /// </summary>
    public class AssetModelPropertyTemplate : AssetModelTemplateBase {

        /// <summary>
        /// The default unit of measure for the template.
        /// </summary>
        public string UnitOfMeasure { get; set; }

    }
}
