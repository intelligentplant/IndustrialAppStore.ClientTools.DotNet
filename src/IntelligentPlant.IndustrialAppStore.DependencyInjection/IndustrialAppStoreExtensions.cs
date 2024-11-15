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
            
            var builder = new IndustrialAppStoreBuilder(services)
                .AddHttpFactory<DefaultIndustrialAppStoreHttpFactory>()
                .AddApiClient();

            builder.Services.TryAddScoped<AccessTokenProvider>();

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
        /// <remarks>
        /// 
        /// <para>
        ///   Note that calling <see cref="HttpClientBuilderExtensions.ConfigureHttpClient(IHttpClientBuilder, Action{HttpClient})"/> 
        ///   or <see cref="HttpClientBuilderExtensions.ConfigureHttpClient(IHttpClientBuilder, Action{IServiceProvider, HttpClient})"/> 
        ///   in your <paramref name="configureHttpBuilder"/> delegate has no effect by default. 
        ///   The final <see cref="HttpClient"/> instance is created by the <see cref="IIndustrialAppStoreHttpFactory"/> 
        ///   service and the default implementation of this service uses <see cref="IHttpMessageHandlerFactory"/> 
        ///   to create an <see cref="HttpMessageHandler"/> and then creates its own <see cref="HttpClient"/> 
        ///   wrapper around the handler.
        /// </para>
        /// 
        /// <para>
        ///   You can customise the behaviour of the <see cref="HttpClient"/> used by the <see cref="IndustrialAppStoreHttpClient"/> 
        ///   by registering a custom <see cref="IIndustrialAppStoreHttpFactory"/> service.
        /// </para>
        /// 
        /// </remarks>
        public static IIndustrialAppStoreBuilder AddApiClient(this IIndustrialAppStoreBuilder builder, Action<IndustrialAppStoreHttpClientOptions>? configureOptions = null, Action<IHttpClientBuilder>? configureHttpBuilder = null) {
            if (builder == null) {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddOptions<IndustrialAppStoreHttpClientOptions>()
                .Configure(options => configureOptions?.Invoke(options))
                .ValidateDataAnnotations();

            builder.Services.TryAddScoped(provider => {
                var http = provider.GetRequiredService<IIndustrialAppStoreHttpFactory>().CreateClient();
                var iasClientOptions = provider.GetRequiredService<IOptions<IndustrialAppStoreHttpClientOptions>>();
                return ActivatorUtilities.CreateInstance<IndustrialAppStoreHttpClient>(provider, http, iasClientOptions.Value);
            });

            builder.Services.TryAddScoped<DataCoreHttpClient>(provider => provider.GetRequiredService<IndustrialAppStoreHttpClient>());

            var httpBuilder = builder.Services.AddHttpClient(IndustrialAppStoreHttpFactory.HttpClientName);
            configureHttpBuilder?.Invoke(httpBuilder);

            return builder;
        }


        /// <summary>
        /// Registers a scoped <see cref="AccessTokenProvider"/> service with the builder.
        /// </summary>
        /// <param name="builder">
        ///   The builder.
        /// </param>
        /// <param name="factory">
        ///   The access token factory delegate that the <see cref="AccessTokenProvider"/> should 
        ///   use.
        /// </param>
        /// <returns>
        ///   The builder.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="builder"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="factory"/> is <see langword="null"/>.
        /// </exception>
        public static IIndustrialAppStoreBuilder AddAccessTokenProvider(this IIndustrialAppStoreBuilder builder, AccessTokenFactory factory) {
            if (builder == null) {
                throw new ArgumentNullException(nameof(builder));
            }
            if (factory == null) {
                throw new ArgumentNullException(nameof(factory));
            }

            builder.Services.AddScoped(_ => new AccessTokenProvider() { 
                Factory = factory 
            });

            return builder;
        }


        /// <summary>
        /// Registers a scoped <see cref="AccessTokenProvider"/> service with the builder.
        /// </summary>
        /// <param name="builder">
        ///   The builder.
        /// </param>
        /// <param name="implementationFactory">
        ///   A delegate that will create the access token factory for the <see cref="AccessTokenProvider"/>.
        /// </param>
        /// <returns>
        ///   The builder.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="builder"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="implementationFactory"/> is <see langword="null"/>.
        /// </exception>
        public static IIndustrialAppStoreBuilder AddAccessTokenProvider(this IIndustrialAppStoreBuilder builder, Func<IServiceProvider, AccessTokenFactory> implementationFactory) {
            if (builder == null) {
                throw new ArgumentNullException(nameof(builder));
            }
            if (implementationFactory == null) {
                throw new ArgumentNullException(nameof(implementationFactory));
            }

            builder.Services.AddScoped(provider => new AccessTokenProvider() {
                Factory = implementationFactory.Invoke(provider)
            });

            return builder;
        }


        /// <summary>
        /// Registers a scoped <see cref="AccessTokenProvider"/> service with the builder that 
        /// will always return the specified access token.
        /// </summary>
        /// <param name="builder">
        ///   The builder.
        /// </param>
        /// <param name="accessToken">
        ///   The access token to use.
        /// </param>
        /// <returns>
        ///   The builder.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="builder"/> is <see langword="null"/>.
        /// </exception>
        public static IIndustrialAppStoreBuilder AddStaticAccessTokenProvider(this IIndustrialAppStoreBuilder builder, string? accessToken) {
            return builder.AddAccessTokenProvider(AccessTokenProvider.CreateStaticAccessTokenFactory(accessToken));
        }

    }
}
