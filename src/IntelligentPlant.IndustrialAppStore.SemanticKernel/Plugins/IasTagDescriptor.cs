using System.ComponentModel;
using System.Text.Json.Serialization;

namespace IntelligentPlant.IndustrialAppStore.SemanticKernel.Plugins {

    [Description("Describes a tag (i.e. an instrument or sensor) on an Industrial App Store data source.")]
    public record IasTagDescriptor {

        [JsonPropertyName("name")]
        [Description("The name of the tag.")]
        public required string Name { get; init; }

        [JsonPropertyName("description")]
        [Description("The tag description.")]
        public string? Description { get; init; }

        [JsonPropertyName("units")]
        [Description("The unit of measure for the tag.")]
        public string? Units { get; init; }

        [JsonPropertyName("labels")]
        [Description("The labels for the tag. Labels can be used to group similar or related tags together.")]
        public string[]? Labels { get; init; }

        [JsonPropertyName("states")]
        [Description("Some tags define discrete named states. When discrete states are defined, the value of the tag must map to one of the known states.")]
        public IasTagDiscreteState[]? States { get; init; }

    }

}
