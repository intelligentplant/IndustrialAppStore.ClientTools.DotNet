using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace IntelligentPlant.IndustrialAppStore.SemanticKernel.Plugins {

    [Description("Describes a request to read current tag values.")]
    public record ReadCurrentValuesRequest : PluginRequest {

        [JsonPropertyName("data_source_id")]
        [Description("The ID of the Industrial App Store data source to query.")]
        public required string DataSourceId { get; init; }

        [JsonPropertyName("tag_names")]
        [Description("The names of the tags to query.")]
        [MinLength(1)]
        [MaxLength(100)]
        public required string[] TagNames { get; init; }

    }

}
