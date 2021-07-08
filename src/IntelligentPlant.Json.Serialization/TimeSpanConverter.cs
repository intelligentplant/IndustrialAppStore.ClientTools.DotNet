using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IntelligentPlant.Json.Serialization {

    /// <summary>
    /// <see cref="JsonConverter{T}"/> for <see cref="TimeSpan"/>.
    /// </summary>
    public class TimeSpanConverter : JsonConverter<TimeSpan> {

        /// <inheritdoc/>
        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            return TimeSpan.Parse(reader.GetString(), CultureInfo.InvariantCulture);
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options) {
            writer?.WriteStringValue(value.ToString("c", CultureInfo.InvariantCulture));
        }

    }
}
