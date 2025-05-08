namespace IntelligentPlant.DataCore.Client.Model {
    /// <summary>
    /// Describes a property associated with an event message.
    /// </summary>
    public class EventProperty : IEventProperty {

        /// <summary>
        /// Gets or sets the event name.
        /// </summary>
        public string Name { get; set; } = default!;

        /// <summary>
        /// Gets or sets the event value.
        /// </summary>
        public object? Value { get; set; }

        /// <summary>
        /// Gets the value of the property as the specified type.
        /// </summary>
        /// <typeparam name="T">
        ///   The property type.
        /// </typeparam>
        /// <returns>
        ///   The property value, or <see langword="null"/> if the value is not of the specified 
        ///   type or cannot be converted to the type.
        /// </returns>
        public T? GetValueOrDefault<T>() {
            if (Value is T value) {
                return value;
            }

            try {
                if (Value is System.Text.Json.JsonElement jsonElement) {
                    return System.Text.Json.JsonSerializer.Deserialize<T>(jsonElement);
                }

                if (Value is Newtonsoft.Json.Linq.JToken jToken) {
                    return jToken.ToObject<T>();
                }

                if (Value is IConvertible convertible) {
                    return (T?) Convert.ChangeType(convertible, typeof(T));
                }
            }
            catch (Exception) {
                // Ignore deserialization exceptions
            }

            return default;
        }


        /// <summary>
        /// Gets the value of the property as the specified type.
        /// </summary>
        /// <typeparam name="T">
        ///   The property type.
        /// </typeparam>
        /// <param name="defaultValue">
        ///   The value to return if the property is undefined, or if it cannot be converted to 
        ///   <typeparamref name="T"/>.
        /// </param>
        /// <returns>
        ///   The property value, or <paramref name="defaultValue"/> if the value is not of the 
        ///   specified type or cannot be converted to the type.
        /// </returns>
        public T? GetValueOrDefault<T>(T defaultValue) {
            if (Value is T value) {
                return value;
            }

            try {
                if (Value is System.Text.Json.JsonElement jsonElement) {
                    return System.Text.Json.JsonSerializer.Deserialize<T>(jsonElement);
                }

                if (Value is Newtonsoft.Json.Linq.JToken jToken) {
                    return jToken.ToObject<T>();
                }

                if (Value is IConvertible convertible) {
                    return (T?) Convert.ChangeType(convertible, typeof(T));
                }
            }
            catch (Exception) {
                // Ignore deserialization exceptions
            }

            return defaultValue;
        }

    }
}
