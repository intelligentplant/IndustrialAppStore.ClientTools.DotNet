using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.DataCore.Client.Queries {
    public class FindAssetModelElementsRequest : DataSourceRequest {

        public string ParentId { get; set; }

        [MaxLength(200)]
        public string NameFilter { get; set; }

        [MaxLength(200)]
        public string PropertyNameFilter { get; set; }

        public bool LoadProperties { get; set; }

        public bool LoadChildren { get; set; }

    }
}
