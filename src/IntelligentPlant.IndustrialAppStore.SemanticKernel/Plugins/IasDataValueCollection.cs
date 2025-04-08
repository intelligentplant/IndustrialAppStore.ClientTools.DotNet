using System.ComponentModel;
using System.Text.Json.Serialization;

namespace IntelligentPlant.IndustrialAppStore.SemanticKernel.Plugins {

    [Description("Describes a collection of data values associated with a tag on an Industrial App Store data source.")]
    public record IasDataValueCollection {

        [JsonPropertyName("name")]
        [Description("The name of the tag associated with the values.")]
        public required string TagName { get; init; }

        [JsonPropertyName("values")]
        [Description("The data values.")]
        public required IasDataValue[] Values { get; init; }

    }

}
