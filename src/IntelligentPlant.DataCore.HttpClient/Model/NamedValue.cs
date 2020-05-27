using System;

namespace IntelligentPlant.DataCore.Client.Model {

    /// <summary>
    /// Describes a named key-value pair.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    public class NamedValue<T> {

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public T Value { get; }


        /// <summary>
        /// Creates a new <see cref="NamedValue{T}"/> object.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        public NamedValue(string name, T value) {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Value = value;
        } 

    }
}
