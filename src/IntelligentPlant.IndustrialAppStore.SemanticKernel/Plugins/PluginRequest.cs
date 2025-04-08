using System.ComponentModel;
using System.Text.Json.Serialization;

namespace IntelligentPlant.IndustrialAppStore.SemanticKernel.Plugins {

    [Description("A basic request to an Industrial App Store plugin.")]
    public record PluginRequest {

        [JsonPropertyName("reason")]
        [Description("A SHORT (1-2 sentences max) plain text explanation for why the agent is calling the function.")]
        public required string Reason { get; init; }

    }

}
