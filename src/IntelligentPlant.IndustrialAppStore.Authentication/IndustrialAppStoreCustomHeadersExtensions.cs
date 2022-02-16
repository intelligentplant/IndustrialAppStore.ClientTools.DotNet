using System;

using IntelligentPlant.IndustrialAppStore.Authentication;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection {

    /// <summary>
    /// Extension methods for custom headers middleware.
    /// </summary>
    public static class IndustrialAppStoreCustomHeadersExtensions {

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

    }
}
