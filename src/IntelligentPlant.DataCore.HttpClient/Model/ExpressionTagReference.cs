using System;

namespace IntelligentPlant.DataCore.Client.Model {

    /// <summary>
    /// Describes a tag reference in an expression.
    /// </summary>
    public class ExpressionTagReference : IEquatable<ExpressionTagReference> {

        /// <summary>
        /// Gets or sets the display name for the reference.
        /// </summary>
        public string DisplayName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the data source name.
        /// </summary>
        public string DataSourceName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the tag name.
        /// </summary>
        public string TagName { get; set; } = default!;

        /// <summary>
        /// Gets or sets a flag stating if the tag reference could be resolved.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Gets or sets the reason that the expression is invalid, when <see cref="IsValid"/> is <see langword="false"/>.
        /// </summary>
        public string? ValidationError { get; set; }

        /// <summary>
        /// Gets a key used in hash code generation and equality comparison.
        /// </summary>
        private string Key {
            get { return string.Concat(DisplayName, ",", DataSourceName, ",", TagName).ToUpperInvariant(); }
        }


        /// <summary>
        /// Gets the hash code for the object.
        /// </summary>
        /// <returns>
        /// The hash code.
        /// </returns>
        public override int GetHashCode() {
            return Key.GetHashCode();
        }


        /// <summary>
        /// Tests of this object is equivalent to another object.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns>
        /// <see langword="true"/> if the objects are equal, otherwise <see langword="false"/>.
        /// </returns>
        public bool Equals(ExpressionTagReference? obj) {
            if (obj == null) {
                return false;
            }

            return Key.Equals(obj.Key);
        }


        /// <summary>
        /// Tests of this object is equivalent to another object.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns>
        /// <see langword="true"/> if the objects are equal, otherwise <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj) {
            return Equals(obj as ExpressionTagReference);
        }

    }

}
