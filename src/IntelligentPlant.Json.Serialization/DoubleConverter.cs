using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IntelligentPlant.Json.Serialization {

    /// <summary>
    /// <see cref="JsonConverter{T}"/> for <see cref="double"/>.
    /// </summary>
    public class DoubleConverter : JsonConverter<double> {

        /// <inheritdoc/>
        public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            if (reader.TokenType == JsonTokenType.Number) {
                return reader.GetDouble();
            }

            if (reader.TokenType == JsonTokenType.String) {
                return double.TryParse(reader.GetString(), out var val)
                    ? val
                    : double.NaN;
            }

            return double.NaN;
        }


        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options) {
            if (double.IsNaN(value) || double.IsInfinity(value)) {
                writer.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
                return;
            }

            writer.WriteNumberValue(value);
        }

    }
}
