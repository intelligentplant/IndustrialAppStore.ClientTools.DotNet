using System.ComponentModel.DataAnnotations;

namespace ExampleMvcApplication.Models {
    public class FindTagsRequest {

        [Required]
        public string DataSourceName { get; set; } = default!;

        [MaxLength(200)]
        public string? TagNameFilter { get; set; }

        public int Page { get; set; } = 1;

    }
}
