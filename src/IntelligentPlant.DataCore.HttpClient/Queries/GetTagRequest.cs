using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.DataCore.Client.Queries {
    public class GetTagRequest : DataSourceRequest {

        [Required]
        public string TagNameOrId { get; set; } = default!;

    }
}
