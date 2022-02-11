using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace IntelligentPlant.DataCore.Client.Model {

    /// <summary>
    /// Describes contact information for a person. See https://docs.npmjs.com/files/package.json#people-fields-author-contributors
    /// for details. 
    /// </summary>
    public class Person {

        /// <summary>
        /// Regex for matching a package.json person string. Modified from https://github.com/jonschlinkert/author-regex.
        /// </summary>
        private static readonly Regex PersonStringRegex = new Regex(@"^(?<name>[^<(]+?)?[ \t]*(?:<(?<email>[^>(]+?)>)?[ \t]*(?:\((?<url>[^)]+?)\)|$)");

        /// <summary>
        /// Gets or sets the person's name.
        /// </summary>
        [Required]
        public string Name { get; set; } = default!;

        /// <summary>
        /// Gets or sets the person's email address.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the person's URL.
        /// </summary>
        public Uri? Url { get; set; }


        /// <summary>
        /// Tries to create a new <see cref="Person"/> from a <c>package.json</c> 
        /// person string. See https://docs.npmjs.com/files/package.json#people-fields-author-contributors 
        /// for details.
        /// </summary>
        /// <param name="personString">The string to create the <see cref="Person"/> from.</param>
        /// <param name="result">The resulting <see cref="Person"/>.</param>
        /// <returns>
        /// A flag that indicates if <paramref name="result"/> was successfully created. When <see langword="false"/>, 
        /// <paramref name="result"/> will be <see langword="null"/>.
        /// </returns>
        public static bool TryCreateFromPersonString(string personString, out Person? result) {
            if (String.IsNullOrWhiteSpace(personString)) {
                result = null;
                return false;
            }

            var m = PersonStringRegex.Match(personString);
            if (!m.Success) {
                result = null;
                return false;
            }

            result = new Person() {
                Name = m.Groups["name"].Value,
                Email = m.Groups["email"].Value,
                Url = Uri.TryCreate(m.Groups["url"].Value, UriKind.Absolute, out var url)
                    ? url
                    : null
            };
            return true;
        }

    }
}
