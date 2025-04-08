using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace IntelligentPlant.IndustrialAppStore.SemanticKernel.Plugins {

    [Description("Describes a request to read a best-fit of raw historical tag values for display in a chart.")]
    public record ReadBestFitHistoricalValuesRequest : PluginRequest {

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

        [JsonPropertyName("width")]
        [Description("The expected pixel width of the chart. A higher number will result in more samples being selected.")]
        [Range(1, 5000)]
        public int Width { get; init; } = 2000;

    }

}
