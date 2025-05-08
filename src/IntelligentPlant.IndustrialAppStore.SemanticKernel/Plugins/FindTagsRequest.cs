using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace IntelligentPlant.IndustrialAppStore.SemanticKernel.Plugins {

    [Description("Describes a request to search for tags on a data source.")]
    public record FindTagsRequest : PluginRequest {

        [JsonPropertyName("data_source_id")]
        [Description("The ID of the Industrial App Store data source to query.")]
        public required string DataSourceId { get; init; }

        [JsonPropertyName("name_filter")]
        [Description("Filters by name. Search terms may use * as a wildcard")]
        [MaxLength(100)]
        public string? Name { get; init; }

        [JsonPropertyName("description_filter")]
        [Description("Filters by description. Search terms may use * as a wildcard")]
        [MaxLength(100)]
        public string? Description { get; init; }

        [JsonPropertyName("units_filter")]
        [Description("Filters by unit of measure. Search terms may use * as a wildcard")]
        [MaxLength(100)]
        public string? Units { get; init; }

        [JsonPropertyName("labels_filter")]
        [Description("Filters by label. Labels can consist of alphanumeric characters, underscores and dashes. Specify multiple labels to match by space-delimiting the individual terms. Search terms may use * as a wildcard")]
        [MaxLength(100)]
        public string? Labels { get; init; }

        [JsonPropertyName("page")]
        [Description("The default page number is 1.")]
        public int Page { get; init; } = 1;

        [JsonPropertyName("page_size")]
        [Description("The default page size is 20.")]
        [Range(1, 100)]
        public int PageSize { get; init; } = 20;

    }

}
