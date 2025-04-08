using System.ComponentModel;
using System.Text.Json.Serialization;

namespace IntelligentPlant.IndustrialAppStore.SemanticKernel.Plugins {

    [Description("Describes a request to search for tags on a data source.")]
    public record FindTagsRequest : PluginRequest {

        [JsonPropertyName("data_source_id")]
        [Description("The ID of the Industrial App Store data source to query.")]
        public required string DataSourceId { get; init; }

        [JsonPropertyName("name_filter")]
        [Description("The tag name filter. The '*' character can be used as a multi-character wildcard e.g. '*.sensor-1.*' returns tags containing '.sensor-1.' in their name.")]
        public string? Name { get; init; }

        [JsonPropertyName("page")]
        [Description("The results page to return. Specify 1 for the first page matching the search terms.")]
        public int Page { get; init; } = 1;

    }

}
