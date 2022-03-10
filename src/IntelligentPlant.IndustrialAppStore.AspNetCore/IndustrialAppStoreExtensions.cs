using IntelligentPlant.IndustrialAppStore.AspNetCore;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection {

    /// <summary>
    /// Extension methods for custom middleware and services.
    /// </summary>
    public static class IndustrialAppStoreExtensions {

        #region [ Custom Headers ]

        /// <summary>
        /// Adds services for custom headers to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">
        ///   The <see cref="IServiceCollection"/>.
        /// </param>
        /// <returns>
        ///   The <see cref="IServiceCollection"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="services"/> is <see langword="null"/>.
        /// </exception>
        public static IServiceCollection AddCustomHeaders(this IServiceCollection services) {
            if (services == null) { 
                throw new ArgumentNullException(nameof(services));
            }

            services.TryAddSingleton<CustomHeadersProvider>();

            return services;
        }


        /// <summary>
        /// Adds a middleware to the pipeline that will add custom headers to every response.
        /// </summary>
        /// <param name="app">
        ///   The <see cref="IApplicationBuilder"/>.
        /// </param>
        /// <returns>
        ///   The <see cref="IApplicationBuilder"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="app"/> is <see langword="null"/>.
        /// </exception>
        /// <remarks>
        ///   Custom headers are configured via the <c>CustomHeaders</c> configuration section.
        /// </remarks>
        public static IApplicationBuilder UseCustomHeaders(this IApplicationBuilder app) {
            if (app == null) {
                throw new ArgumentNullException(nameof(app));
            }

            app.UseMiddleware<CustomHeadersMiddleware>();

            return app;
        }

        #endregion

        #region [ Content Security Policy ]

        /// <summary>
        /// Adds services required for adding a Content Security Policy to HTTP responses.
        /// </summary>
        /// <param name="services">
        ///   The <see cref="IServiceCollection"/>.
        /// </param>
        /// <returns>
        ///   The <see cref="IServiceCollection"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="services"/> is <see langword="null"/>.
        /// </exception>
        public static IServiceCollection AddContentSecurityPolicy(this IServiceCollection services) {
            if (services == null) {
                throw new ArgumentNullException(nameof(services));
            }

            services.TryAddSingleton<ContentSecurityPolicyProvider>();
            services.TryAddScoped<ContentSecurityPolicyBuilder>();

            return services;
        }


        /// <summary>
        /// Adds middleware that will build a Content Security Policy for the current HTTP context.
        /// </summary>
        /// <param name="app">
        ///   The <see cref="IApplicationBuilder"/>.
        /// </param>
        /// <param name="configure">
        ///   AN optional delegate that can be used to customise the Content Security Policy 
        ///   generated from the default policy definitions.
        /// </param>
        /// <returns>
        ///   The <see cref="IApplicationBuilder"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="app"/> is <see langword="null"/>.
        /// </exception>
        public static IApplicationBuilder UseContentSecurityPolicy(this IApplicationBuilder app, Func<HttpContext, ContentSecurityPolicyBuilder, Task>? configure = null) {
            if (app == null) {
                throw new ArgumentNullException(nameof(app));
            }

            app.Use(async (ctx, next) => {
                ctx.Response.OnStarting(ApplyContentSecurityPolicy, Tuple.Create(ctx, configure));
                await next.Invoke().ConfigureAwait(false);
            });

            return app;
        }


        /// <summary>
        /// Generates the content security policy for an HTTP response.
        /// </summary>
        /// <param name="state">
        ///   The state for the callback.
        /// </param>
        /// <returns>
        ///   A <see cref="Task"/> that will generate the CSP.
        /// </returns>
        private static async Task ApplyContentSecurityPolicy(object state) {
            var tuple = (Tuple<HttpContext, Func<HttpContext, ContentSecurityPolicyBuilder, Task>?>) state;
            var ctx = tuple.Item1;
            var configure = tuple.Item2;

            if (string.IsNullOrWhiteSpace(ctx.Response.ContentType)) {
                return;
            }

            // Only send CSP response header with text/html content
            if (new System.Net.Mime.ContentType(ctx.Response.ContentType).MediaType.Equals("text/html", StringComparison.OrdinalIgnoreCase)) {
                var policyProvider = ctx.RequestServices.GetRequiredService<ContentSecurityPolicyProvider>();
                var policyBuilder = policyProvider.CreatePolicyBuilder(ctx.Request.Path);

                if (configure != null) {
                    await configure.Invoke(ctx, policyBuilder).ConfigureAwait(false);
                }

                var policy = policyBuilder.Build();
                if (!string.IsNullOrWhiteSpace(policy)) {
#if NET6_0_OR_GREATER
                    if (policyBuilder.ReportOnly) {
                        ctx.Response.Headers.ContentSecurityPolicyReportOnly = policy;
                    }
                    else {
                        ctx.Response.Headers.ContentSecurityPolicy = policy;
                    }
#else
                    if (policyBuilder.ReportOnly) {
                        ctx.Response.Headers.Add("Content-Security-Policy-Report-Only", policy);
                    }
                    else {
                        ctx.Response.Headers.Add("Content-Security-Policy", policy);
                    }
#endif
                }
            }
        }

#endregion

    }
}
