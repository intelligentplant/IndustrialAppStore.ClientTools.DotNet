using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace IntelligentPlant.IndustrialAppStore.SemanticKernel.Plugins {

    [Description("Describes a request to read raw historical tag values.")]
    public record ReadRawHistoricalValuesRequest : PluginRequest {

        [JsonPropertyName("data_source_id")]
        [Description("The ID of the Industrial App Store data source to query.")]
        public required string DataSourceId { get; init; }

        [JsonPropertyName("tag_names")]
        [Description("The names of the tags to query.")]
        [MinLength(1)]
        [MaxLength(20)]
        public required string[] TagNames { get; init; }

        [JsonPropertyName("start_time")]
        [Description("The absolute or relative start time for the query.")]
        public required string StartTime { get; init; }

        [JsonPropertyName("end_time")]
        [Description("The absolute or relative end time for the query.")]
        public required string EndTime { get; init; }

        [JsonPropertyName("max_samples")]
        [Description("The maximum number of samples to retrieve per tag.")]
        [Range(1, 5000)]
        public int MaxSamples { get; init; } = 5000;

    }

}
