using System.ComponentModel.DataAnnotations;

namespace ExampleMvcApplication.Models {
    public class GetChartDataRequest {

        [Required]
        public string DataSourceName { get; set; }

        [Required]
        public string TagName { get; set; }

    }
}
