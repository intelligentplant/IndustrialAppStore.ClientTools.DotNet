using System;
using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.DataCore.Client.Model {
    /// <summary>
    /// Describes the name of a Data Core component.
    /// </summary>
    public class ComponentName {

        /// <summary>
        /// Gets the unqualified component name.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        /// <summary>
        /// Gets the component namespace.
        /// </summary>
        [MaxLength(200)]
        public string Namespace { get; set; }

        /// <summary>
        /// Gets the qualified component name.
        /// </summary>
        public string QualifiedName { get { return GetQualifiedName(Name, Namespace); } }

        /// <summary>
        /// The display name for the component.
        /// </summary>
        private string _displayName;

        /// <summary>
        /// Gets the display name for the component.
        /// </summary>
        [MaxLength(200)]
        public string DisplayName {
            get { return String.IsNullOrWhiteSpace(_displayName) ? QualifiedName : _displayName; }
            set { _displayName = value; }
        }


        /// <summary>
        /// Gets the qualified name for the component with the specified name and namespace.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="namespace">The namespace.</param>
        /// <returns>
        /// The qualified name, or <see langword="null"/> if <paramref name="name"/> is <see langword="null"/>.
        /// </returns>
        public static string GetQualifiedName(string name, string @namespace) {
            if (String.IsNullOrWhiteSpace(name)) {
                return null;
            }

            return String.IsNullOrWhiteSpace(@namespace)
                       ? name
                       : String.Concat(@namespace, ".", name);
        }



    }
    
}
