using System.Linq;
using System.Text.Json.Serialization;

using IntelligentPlant.Json.Serialization;

namespace System.Text.Json {

    /// <summary>
    /// Extensions for <see cref="JsonSerializerOptions"/>.
    /// </summary>
    public static class IntelligentPlantJsonSerializationExtensions {

        /// <summary>
        /// Adds a <see cref="JsonConverter"/> to the <see cref="JsonSerializerOptions"/> if an 
        /// existing converter of the same type has not already been added to the 
        /// <see cref="JsonSerializerOptions.Converters"/> collection.
        /// </summary>
        /// <typeparam name="T">
        ///   The JSON converter type to add.
        /// </typeparam>
        /// <param name="options">
        ///   The <see cref="JsonSerializerOptions"/>.
        /// </param>
        /// <returns>
        ///   The <see cref="JsonSerializerOptions"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="options"/> is <see langword="null"/>.
        /// </exception>
        public static JsonSerializerOptions TryAddConverter<T>(this JsonSerializerOptions options) where T : JsonConverter, new() {
            if (options == null) {
                throw new ArgumentNullException(nameof(options));
            }

            if (!options.Converters.OfType<T>().Any()) {
                options.Converters.Add(new T());
            }

            return options;
        }


        /// <summary>
        /// Adds the default set of Intelligent Plant converters to the <see cref="JsonSerializerOptions"/>.
        /// </summary>
        /// <param name="options">
        ///   The <see cref="JsonSerializerOptions"/>.
        /// </param>
        /// <returns>
        ///   The <see cref="JsonSerializerOptions"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="options"/> is <see langword="null"/>.
        /// </exception>
        /// <remarks>
        /// 
        /// <para>
        ///   The following converters will be added to the <paramref name="options"/>:
        /// </para>
        /// 
        /// <list type="bullet">
        ///   <item>
        ///     <see cref="DoubleConverter"/>
        ///   </item>
        ///   <item>
        ///     <see cref="FloatConverter"/>
        ///   </item>
        ///   <item>
        ///     <see cref="TimeSpanConverter"/>
        ///   </item>
        /// </list>
        /// 
        /// </remarks>
        public static JsonSerializerOptions AddIntelligentPlantConverters(this JsonSerializerOptions options) {
            if (options == null) {
                throw new ArgumentNullException(nameof(options));
            }

            options.TryAddConverter<DoubleConverter>();
            options.TryAddConverter<FloatConverter>();
            options.TryAddConverter<TimeSpanConverter>();

            return options;
        }

    }
}
