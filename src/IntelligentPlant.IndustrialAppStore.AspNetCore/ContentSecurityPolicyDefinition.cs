
namespace IntelligentPlant.IndustrialAppStore.AspNetCore {

    /// <summary>
    /// Defines a set of Content Security Policy directives.
    /// </summary>
    /// <remarks>
    ///   Note that a single HTTP request may match multiple policy definitions. When multiple 
    ///   definitions are matched, the results are additive by default. See the notes for the 
    ///   <see cref="Policy"/> property for details on how to remove directive values inherited 
    ///   from other <see cref="ContentSecurityPolicyDefinition"/> instances.
    /// </remarks>
    public class ContentSecurityPolicyDefinition {

        /// <summary>
        /// The priority for the definition. Higher value policies will be applied first in the 
        /// event that multiple policies match a given path. 
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// An optional set of paths that the definition matches against.
        /// </summary>
        /// <remarks>
        /// 
        /// <para>
        ///   If <see cref="Match"/> is <see langword="null"/>, the policy definition will match 
        ///   all incoming requests.
        /// </para>
        /// 
        /// <para>
        ///   Each entry in <see cref="Match"/> may contain wildcards e.g. <c>/home/*</c>.
        /// </para>
        /// 
        /// </remarks>
        public string[]? Match { get; set; }

        /// <summary>
        /// The Content Security Policy directives and values.
        /// </summary>
        /// <remarks>
        /// 
        /// <para>
        ///   The format for each directive value is <c>[+:]value</c> or <c>-:value</c>, with the 
        ///   former used to add values to the directive and the latter used to remove values 
        ///   from the directive that were added by a higher-priority policy definition.
        /// </para>
        /// 
        /// <para>
        ///   For example:
        /// </para>
        /// 
        /// <code lang="json">
        /// {
        ///   "default-src": [
        ///     "'self'",
        ///     "-:'unsafe-inline'"
        ///     "+:https://*.intelligentplant.com"
        ///   ]
        /// }
        /// </code>
        /// 
        /// </remarks>
        public IDictionary<string, string[]>? Policy { get; set; }

    }
}
