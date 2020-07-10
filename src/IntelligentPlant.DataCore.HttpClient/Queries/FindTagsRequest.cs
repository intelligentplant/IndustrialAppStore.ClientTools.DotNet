using System.ComponentModel.DataAnnotations;

using IntelligentPlant.DataCore.Client.Model;

namespace IntelligentPlant.DataCore.Client.Queries {
    public class FindTagsRequest : DataSourceRequest {

        [Required]
        public TagSearchFilter Filter { get; set; }

    }
}
