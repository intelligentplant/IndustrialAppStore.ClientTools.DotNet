using IntelligentPlant.DataCore.Client.Model;

namespace ExampleMvcApplication.Models {
    public class TagListViewModel {

        public FindTagsRequest Request { get; set; }

        public IEnumerable<TagSearchResult> Tags { get; set; } = Array.Empty<TagSearchResult>();

        public bool CanPageNext { get; set; }

    }
}
