using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.DataCore.Client.Queries {
    public class DataSourceRequest {

        [Required]
        public string DataSourceName { get; set; } = default!;

    }
}
