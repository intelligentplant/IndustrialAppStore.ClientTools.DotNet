using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.DataCore.Client.Queries {

    /// <summary>
    /// A request to retrieve an asset model element template.
    /// </summary>
    public class GetAssetModelElementTemplateRequest : DataSourceRequest {

        /// <summary>
        /// The template ID.
        /// </summary>
        [Required]
        public string Id { get; set; }

    }

}
