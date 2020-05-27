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
        /// Creates a new <see cref="ComponentName"/> using the specified <paramref name="name"/>, <paramref name="namespace"/> and <paramref name="displayName"/>.
        /// </summary>
        /// <param name="name">The component name.</param>
        /// <param name="namespace">The component namespace.</param>
        /// <param name="displayName">The display name.</param>
        public ComponentName(string name, string @namespace, string displayName) {
            Name = name;
            Namespace = @namespace;
            DisplayName = displayName;
        }


        /// <summary>
        /// Creates a new <see cref="ComponentName"/> object.
        /// </summary>
        public ComponentName() : this(null, null, null) { }

        /// <summary>
        /// Creates a new <see cref="ComponentName"/> object from a single name string. Examine the name string syntax to determine if it is namespace qualified.
        /// </summary>
        /// <param name="name">The str</param>
        public ComponentName(string name)
        {
            var nameParts = name.Split(new char['.'], 1);
            if (nameParts.Length > 1)
            {
                Namespace = nameParts[0];
                Name = nameParts[1];
                DisplayName = nameParts[1];
            }
            else
            {
                Name = name;
                DisplayName = name;
            }
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
