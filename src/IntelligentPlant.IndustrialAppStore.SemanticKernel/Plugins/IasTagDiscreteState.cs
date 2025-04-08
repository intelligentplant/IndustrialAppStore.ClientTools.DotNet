using System.ComponentModel;
using System.Text.Json.Serialization;

namespace IntelligentPlant.IndustrialAppStore.SemanticKernel.Plugins {

    [Description("Describes a named discrete state associated with a tag on an Industrial App Store data source.")]
    public record IasTagDiscreteState {

        [JsonPropertyName("name")]
        [Description("The name of the discrete state.")]
        public required string Name { get; init; }

        [JsonPropertyName("value")]
        [Description("The value for the state.")]
        public required int Value { get; init; }

    }

}
