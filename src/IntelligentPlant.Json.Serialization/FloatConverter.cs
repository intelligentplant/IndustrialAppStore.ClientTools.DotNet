using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IntelligentPlant.Json.Serialization {

    /// <summary>
    /// <see cref="JsonConverter{T}"/> for <see cref="float"/>.
    /// </summary>
    public class FloatConverter : JsonConverter<float> {

        /// <inheritdoc/>
        public override float Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            if (reader.TokenType == JsonTokenType.Number) {
                return reader.GetSingle();
            }

            if (reader.TokenType == JsonTokenType.String) {
                return float.TryParse(reader.GetString(), out var val)
                    ? val
                    : float.NaN;
            }

            return float.NaN;
        }


        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, float value, JsonSerializerOptions options) {
            if (float.IsNaN(value) || float.IsInfinity(value)) {
                writer.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
                return;
            }

            writer.WriteNumberValue(value);
        }

    }
}
