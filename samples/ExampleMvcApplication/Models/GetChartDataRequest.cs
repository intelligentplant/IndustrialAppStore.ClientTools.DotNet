using System.ComponentModel.DataAnnotations;

namespace ExampleMvcApplication.Models {
    public class GetChartDataRequest {

        [Required]
        public string DataSourceName { get; set; } = default!;

        [Required]
        public string TagName { get; set; } = default!;

    }
}
