using System.ComponentModel;
using System.Text.Json.Serialization;

using IntelligentPlant.DataCore.Client.Model;

namespace IntelligentPlant.IndustrialAppStore.SemanticKernel.Plugins {

    [Description("Describes a data value associated with a tag on an Industrial App Store data source.")]
    public record IasDataValue {

        [JsonPropertyName("utc_sample_time")]
        [Description("The UTC sample time for the data value.")]
        public required DateTime UtcSampleTime { get; init; }

        [JsonPropertyName("value")]
        [JsonNumberHandling(JsonNumberHandling.AllowNamedFloatingPointLiterals)]
        [Description("The numeric value for the data value. The display_value field should be used for display purposes instead of the value field if it is defined.")]
        public required double Value { get; init; }

        [JsonPropertyName("display_value")]
        [Description("The display value for the data value. The display_value field should be used for display purposes instead of the value field if it is defined.")]
        public string? DisplayValue { get; init; }

        [JsonPropertyName("quality")]
        [Description("The quality associated with the sample (good/uncertain/bad). This indicates the trustworthiness of the value.")]
        public string? Quality { get; init; }


        internal static IasDataValue FromTagValue(TagValue value) {
            return new IasDataValue() {
                UtcSampleTime = value.UtcSampleTime,
                Value = value.IsNumeric ? value.NumericValue : double.NaN,
                DisplayValue = value.TextValue,
                Quality = value.Status switch {
                    TagValueStatus.Good => "good",
                    TagValueStatus.Uncertain => "uncertain",
                    TagValueStatus.Bad => "bad",
                    _ => "unknown"
                }
            };
        }

    }

}
