
namespace IntelligentPlant.IndustrialAppStore.AspNetCore {

    /// <summary>
    /// Options for <see cref="ContentSecurityPolicyProvider"/>.
    /// </summary>
    public class ContentSecurityPolicyOptions {

        /// <summary>
        /// When <see langword="true"/>, the CSP will be set via the <c>Content-Security-Policy-Report-Only</c> 
        /// header instead of the <c>Content-Security-Policy</c> header.
        /// </summary>
        public bool ReportOnly { get; set; }

        /// <summary>
        /// The policy definitions.
        /// </summary>
        public IDictionary<string, ContentSecurityPolicyDefinition>? Policies { get; set; }

    }
}
