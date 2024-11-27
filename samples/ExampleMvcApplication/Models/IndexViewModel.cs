using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExampleMvcApplication.Models {
    public class IndexViewModel {

        public string DataSourceName { get; set; } = default!;

        public IEnumerable<SelectListItem> DataSources { get; set; } = default!;

    }
}
