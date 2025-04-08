using System.ComponentModel;
using System.Text.Json.Serialization;

using IntelligentPlant.DataCore.Client.Model;

namespace IntelligentPlant.IndustrialAppStore.SemanticKernel.Plugins {

    [Description("Describes an Industrial App Store data source.")]
    public record IasDataSourceDescriptor {

        [JsonPropertyName("id")]
        [Description("The unique identifier for the data source.")]
        public required string Id { get; init; }

        [JsonPropertyName("display_name")]
        [Description("The display name for the data source.")]
        public required string DisplayName { get; init; }

        [JsonPropertyName("description")]
        [Description("The description for the data source.")]
        public string? Description { get; init; }

    }

}
