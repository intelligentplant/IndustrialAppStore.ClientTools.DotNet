using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.DataCore.Client.Queries {

    /// <summary>
    /// A request to retrieve property templates defined on an element template.
    /// </summary>
    public class FindAssetModelPropertyTemplatesRequest : FindAssetModelElementTemplatesRequest {

        /// <summary>
        /// The ID of the element template to retrieve the properties for.
        /// </summary>
        [Required]
        public string Id { get; set; } = default!;

    }
}
