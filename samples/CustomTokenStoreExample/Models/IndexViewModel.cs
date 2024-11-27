using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExampleMvcApplication.Models {
    public class IndexViewModel {

        public string DataSourceName { get; set; }

        public IEnumerable<SelectListItem> DataSources { get; set; }

    }
}
