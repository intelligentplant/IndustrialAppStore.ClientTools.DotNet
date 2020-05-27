using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.DataCore.Client.Queries {
    public class GetAssetModelElementRequest : DataSourceRequest {

        [Required]
        public string Id { get; set; }

        public bool LoadProperties { get; set; }

        public bool LoadChildren { get; set; }

    }
}
