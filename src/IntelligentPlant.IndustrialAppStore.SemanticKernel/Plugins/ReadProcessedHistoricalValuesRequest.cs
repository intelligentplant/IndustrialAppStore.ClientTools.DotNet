using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace IntelligentPlant.IndustrialAppStore.SemanticKernel.Plugins {

    [Description("Describes a request to read processed/aggregated historical tag values.")]
    public record ReadProcessedHistoricalValuesRequest : PluginRequest {

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

        [JsonPropertyName("data_function")]
        [Description("The aggregate function to use. Different data sources support different functions but commonly-supported functions include INTERP (interpolated), AVG (average), MIN (minimum) and MAX (maximum).")]
        public required string DataFunction { get; init; }

        [JsonPropertyName("sample_interval")]
        [Description("The sample interval to perform the aggregation over. Note that ISO 8601 time periods are *not* supported.")]
        public required string SampleInterval { get; init; }

    }

}
