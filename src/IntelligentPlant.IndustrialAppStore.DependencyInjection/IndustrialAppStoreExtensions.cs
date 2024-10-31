using System;
using System.Net.Http;

using IntelligentPlant.DataCore.Client;
using IntelligentPlant.IndustrialAppStore.Client;
using IntelligentPlant.IndustrialAppStore.DependencyInjection;
using IntelligentPlant.IndustrialAppStore.DependencyInjection.Internal;

using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection {

    /// <summary>
    /// Extensions for configuring services for the Industrial App Store.
    /// </summary>
    public static class IndustrialAppStoreExtensions {

        /// <summary>
        /// Adds services required for calling the Industrial App Store API.
        /// </summary>
        /// <param name="services">
        ///   The service collection.
        /// </param>
        /// <returns>
        ///   An <see cref="IIndustrialAppStoreBuilder"/> that can be used to configure additional 
        ///   Industrial App Store services.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="services"/> is <see langword="null"/>.
        /// </exception>
        public static IIndustrialAppStoreBuilder AddIndustrialAppStoreApiServices(this IServiceCollection services) {
            if (services == null) {
                throw new ArgumentNullException(nameof(services));
            }
            
            return new IndustrialAppStoreBuilder(services)
                .AddHttpFactory<DefaultIndustrialAppStoreHttpFactory>()
                .AddApiClient();
        }


        /// <summary>
        /// Adds a scoped <see cref="IIndustrialAppStoreHttpFactory"/> service to the builder.
        /// </summary>
        /// <typeparam name="T">
        ///   The <see cref="IIndustrialAppStoreHttpFactory"/> implementation type.
        /// </typeparam>
        /// <param name="builder">
        ///   The <see cref="IIndustrialAppStoreBuilder"/>.
        /// </param>
        /// <returns>
        ///   The <see cref="IIndustrialAppStoreBuilder"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="builder"/> is <see langword="null"/>.
        /// </exception>
        /// <remarks>
        ///   The <see cref="IIndustrialAppStoreHttpFactory"/> service is responsible for creating 
        ///   HTTP handlers for <see cref="IndustrialAppStoreHttpClient"/> instances.
        /// </remarks>
        public static IIndustrialAppStoreBuilder AddHttpFactory<T>(this IIndustrialAppStoreBuilder builder) where T : class, IIndustrialAppStoreHttpFactory {
            if (builder == null) {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddScoped<IIndustrialAppStoreHttpFactory, T>();

            return builder;
        }


        /// <summary>
        /// Adds a scoped <see cref="IIndustrialAppStoreHttpFactory"/> service to the builder.
        /// </summary>
        /// <typeparam name="T">
        ///   The <see cref="IIndustrialAppStoreHttpFactory"/> implementation type.
        /// </typeparam>
        /// <param name="builder">
        ///   The <see cref="IIndustrialAppStoreBuilder"/>.
        /// </param>
        /// <param name="implementationFactory">
        ///   The factory that creates the service.
        /// </param>
        /// <returns>
        ///   The <see cref="IIndustrialAppStoreBuilder"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="builder"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="implementationFactory"/> is <see langword="null"/>.
        /// </exception>
        /// <remarks>
        ///   The <see cref="IIndustrialAppStoreHttpFactory"/> service is responsible for creating 
        ///   HTTP handlers for <see cref="IndustrialAppStoreHttpClient"/> instances.
        /// </remarks>
        public static IIndustrialAppStoreBuilder AddHttpFactory<T>(this IIndustrialAppStoreBuilder builder, Func<IServiceProvider, T> implementationFactory) where T : class, IIndustrialAppStoreHttpFactory {
            if (builder == null) {
                throw new ArgumentNullException(nameof(builder));
            }
            if (implementationFactory == null) {
                throw new ArgumentNullException(nameof(implementationFactory));
            }

            builder.Services.AddScoped<IIndustrialAppStoreHttpFactory>(implementationFactory);

            return builder;
        }


        /// <summary>
        /// Registers a scoped <see cref="IndustrialAppStoreHttpClient"/> service with the builder.
        /// </summary>
        /// <param name="builder">
        ///   The builder.
        /// </param>
        /// <param name="configureOptions">
        ///   A delegate that is used to configure the options for the <see cref="IndustrialAppStoreHttpClient"/>.
        /// </param>
        /// <param name="configureHttpBuilder">
        ///   A delegate that is used to configure the HTTP client for the <see cref="IndustrialAppStoreHttpClient"/> 
        ///   service.
        /// </param>
        /// <returns>
        ///   The builder.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="builder"/> is <see langword="null"/>.
        /// </exception>
        public static IIndustrialAppStoreBuilder AddApiClient(this IIndustrialAppStoreBuilder builder, Action<IndustrialAppStoreHttpClientOptions>? configureOptions = null, Action<IHttpClientBuilder>? configureHttpBuilder = null) {
            if (builder == null) {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddOptions<IndustrialAppStoreHttpClientOptions>().Configure(options => configureOptions?.Invoke(options));

            builder.Services.TryAddScoped(provider => {
                var httpHandler = provider.GetRequiredService<IIndustrialAppStoreHttpFactory>().CreateHandler();
                var http = new HttpClient(httpHandler, false);
#if NET8_0_OR_GREATER
                http.DefaultRequestVersion = System.Net.HttpVersion.Version11;
                http.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
#endif
                var iasClientOptions = provider.GetRequiredService<IOptions<IndustrialAppStoreHttpClientOptions>>();
                return ActivatorUtilities.CreateInstance<IndustrialAppStoreHttpClient>(provider, http, iasClientOptions.Value);
            });

            builder.Services.TryAddScoped<DataCoreHttpClient>(provider => provider.GetRequiredService<IndustrialAppStoreHttpClient>());

            var httpBuilder = builder.Services.AddHttpClient(nameof(IndustrialAppStoreHttpClient));
            configureHttpBuilder?.Invoke(httpBuilder);

            return builder;
        }

    }
}
