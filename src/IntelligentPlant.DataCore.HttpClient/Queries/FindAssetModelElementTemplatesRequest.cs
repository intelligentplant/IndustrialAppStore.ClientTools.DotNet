using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.DataCore.Client.Queries {

    /// <summary>
    /// A request to retrieve element templates from an asset model.
    /// </summary>
    public class FindAssetModelElementTemplatesRequest : DataSourceRequest {

        /// <summary>
        /// The template name filter to apply.
        /// </summary>
        [MaxLength(200)]
        public string? NameFilter { get; set; }

    }
}
