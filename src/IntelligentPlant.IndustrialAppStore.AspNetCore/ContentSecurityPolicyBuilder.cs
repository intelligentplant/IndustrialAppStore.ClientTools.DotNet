using System.Collections.Concurrent;

namespace IntelligentPlant.IndustrialAppStore.AspNetCore {

    /// <summary>
    /// Class for constructing a <c>Content-Security-Policy</c>.
    /// </summary>
    public class ContentSecurityPolicyBuilder {

        /// <summary>
        /// The configured policy directives and values.
        /// </summary>
        private readonly ConcurrentDictionary<string, HashSet<string>> _policyDirectives = new ConcurrentDictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// When <see langword="true"/>, the policy is intended to be used in the <c>Content-Security-Policy-Report-Only</c> 
        /// header and not the <c>Content-Security-Policy</c> header.
        /// </summary>
        public bool ReportOnly { get; set; }


        /// <summary>
        /// Gets a collection containing the values for the specified CSP directive, adding the 
        /// directive to the builder if required.
        /// </summary>
        /// <param name="name">
        ///   The CSP directive.
        /// </param>
        /// <returns>
        ///   A collection that holds the values for the directive.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="name"/> is <see langword="null"/> or white space.
        /// </exception>
        public ICollection<string> GetOrCreateDirective(string name) {
            if (string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentOutOfRangeException(nameof(name));
            }
            return _policyDirectives.GetOrAdd(name, k => new HashSet<string>(StringComparer.OrdinalIgnoreCase));
        }


        /// <summary>
        /// Deletes the specified CSP directive and its associated values from the builder.
        /// </summary>
        /// <param name="name">
        ///   The CSP directive.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="name"/> is <see langword="null"/> or white space.
        /// </exception>
        public void DeleteDirective(string name) {
            if (string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentOutOfRangeException(nameof(name));
            }
            _policyDirectives.TryRemove(name, out _);
        }


        /// <summary>
        /// Removes all directives and values from the builder.
        /// </summary>
        public void Clear() {
            _policyDirectives.Clear();
        }


        /// <summary>
        /// Builds the Content Security Policy from the configured directives and values.
        /// </summary>
        /// <returns>
        ///   The Content Security Policy.
        /// </returns>
        public string Build() {
            return string.Join("; ", _policyDirectives.Select(x => $"{x.Key} {string.Join(" ", x.Value.Where(v => !string.IsNullOrWhiteSpace(v)))}"));
        }

    }
}
