using Microsoft.AspNetCore.Http;

namespace IntelligentPlant.IndustrialAppStore.AspNetCore {

    /// <summary>
    /// A delegate that configures the content security policy for the current request.
    /// </summary>
    /// <param name="context">
    ///   The HTTP context for the request.
    /// </param>
    /// <param name="builder">
    ///   The CSP builder for the request.
    /// </param>
    /// <returns>
    ///   A <see cref="ValueTask"/> that will configure the CSP.
    /// </returns>
    public delegate ValueTask ContentSecurityPolicyConfigurationDelegate(HttpContext context, ContentSecurityPolicyBuilder builder);

}
