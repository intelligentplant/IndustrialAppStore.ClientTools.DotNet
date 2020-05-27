using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.DataCore.Client.Queries {
    public class IsAuthorizedOnDataSourceRequest : DataSourceRequest {

        [Required]
        [MinLength(1)]
        public string[] RoleNames { get; set; }

    }
}
