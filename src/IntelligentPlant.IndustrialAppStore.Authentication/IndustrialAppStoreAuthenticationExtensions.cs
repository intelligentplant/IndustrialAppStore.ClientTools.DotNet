using System;
using System.Net.Http;

using IntelligentPlant.IndustrialAppStore.Authentication;
using IntelligentPlant.IndustrialAppStore.Authentication.Http;
using IntelligentPlant.IndustrialAppStore.Authentication.Options;
using IntelligentPlant.IndustrialAppStore.Client;
using IntelligentPlant.IndustrialAppStore.DependencyInjection;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection {

    /// <summary>
    /// Authentication extensions for the Intelligent Plant Industrial App Store.
    /// </summary>
    public static class IndustrialAppStoreAuthenticationExtensions {

        /// <summary>
        /// Default client secret to use when configuring OAuth authentication options if one has 
        /// not been provided. This will not actually be used for authentication by the token 
        /// server; it is just that the ASP.NET Core OAuth authentication options require that a 
        /// secret is set at this end.
        /// </summary>
        internal const string DefaultClientSecret = "IndustrialAppStore";


        /// <summary>
        /// Adds Industrial App Store authentication and associated services to the application.
        /// </summary>
        /// <param name="services">
        ///   The service collection.
        /// </param>
        /// <param name="configure">
        ///   A delegate that can be used to configure the authentication options.
        /// </param>
        /// <returns>
        ///   An <see cref="IIndustrialAppStoreBuilder"/> that can be used to perform additional 
        ///   customisation of the Industrial App Store services.
        /// </returns>
        /// <remarks>
        /// 
        /// <para>
        ///   Calling this method will register the following services:
        /// </para>
        /// 
        /// <list type="bullet">
        ///   <item>
        ///     Cookie and OAuth authentication schemes (unless <see cref="IndustrialAppStoreAuthenticationOptions.UseExternalAuthentication"/> 
        ///     is <see langword="true"/>).
        ///   </item>
        ///   <item>
        ///     A scoped <see cref="IndustrialAppStoreHttpClient"/> service that can be used to 
        ///     query the Industrial App Store API and the Data API.
        ///   </item>
        ///   <item>
        ///     A scoped <see cref="ITokenStore"/> service that uses the authentication session 
        ///     (i.e. the caller's encrypted session cookie) to store tokens.
        ///   </item>
        ///   <item>
        ///     A scoped <see cref="IIndustrialAppStoreHttpFactory"/> implementation that uses the 
        ///     registered <see cref="ITokenStore"/> to retrieve the access token that the 
        ///     <see cref="IndustrialAppStoreHttpClient"/> will use to authenticate outgoing 
        ///     requests.
        ///   </item>
        ///   <item>
        ///     An HTTP client factory registration for use with <see cref="IndustrialAppStoreHttpClient"/> 
        ///     that uses the standard resilience handler configured by <see cref="ResilienceHttpClientBuilderExtensions.AddStandardResilienceHandler(IHttpClientBuilder)"/>.
        ///   </item>
        /// </list>
        /// 
        /// <para>
        ///   The <see cref="IIndustrialAppStoreBuilder"/> returned by this method can by used to 
        ///   perform any additional configuration required by the application:
        /// </para>
        /// 
        /// <list type="bullet">
        ///   <item>
        ///     To register a custom <see cref="ITokenStore"/> service, call <see cref="AddTokenStore{T}(IIndustrialAppStoreBuilder)"/> 
        ///     or <see cref="AddTokenStore{T}(IIndustrialAppStoreBuilder, Func{IServiceProvider, HttpClient, T})"/>.
        ///   </item>
        ///   <item>
        ///     To customise the HTTP client registration used by <see cref="IndustrialAppStoreHttpClient"/>, call 
        ///     <see cref="IndustrialAppStoreExtensions.AddApiClient(IIndustrialAppStoreBuilder, Action{IHttpClientBuilder})"/>.
        ///   </item>
        /// </list>
        /// 
        /// </remarks>
        public static IIndustrialAppStoreBuilder AddIndustrialAppStoreAuthentication(this IServiceCollection services, Action<IndustrialAppStoreAuthenticationOptions> configure) {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configure);

            return services.AddIndustrialAppStoreServices()
                .AddHttpFactory<TokenStoreHttpFactory>()
                .AddApiClient(http => http
                    .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler() {
                        EnableMultipleHttp2Connections = true
                    })
                    .AddStandardResilienceHandler())
                .AddCoreAuthenticationServices()
                .AddAuthentication(configure);
        }


        /// <summary>
        /// Adds core services used by Industrial App Store authentication.
        /// </summary>
        /// <param name="builder">
        ///   The <see cref="IIndustrialAppStoreBuilder"/>.
        /// </param>
        /// <returns>
        ///   The <see cref="IIndustrialAppStoreBuilder"/>.
        /// </returns>
        internal static IIndustrialAppStoreBuilder AddCoreAuthenticationServices(this IIndustrialAppStoreBuilder builder) {
            // HTTP client registration for backchannel communications with the IAS OAuth token
            // endpoint.
            builder.Services.AddHttpClient("ias_oauth_backchannel").AddStandardResilienceHandler();

            // Default ITokenStore implementation that uses the authentication session to store tokens.
            builder.AddTokenStore<AuthenticationPropertiesTokenStore>();

            return builder;
        }


        /// <summary>
        /// Registers a scoped <see cref="ITokenStore"/> service.
        /// </summary>
        /// <typeparam name="T">
        ///   The <see cref="ITokenStore"/> implementation type.
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
        public static IIndustrialAppStoreBuilder AddTokenStore<T>(this IIndustrialAppStoreBuilder builder) where T : class, ITokenStore {
            return builder.AddTokenStore((provider, http) => ActivatorUtilities.CreateInstance<T>(provider, http));
        }


        /// <summary>
        /// Registers a scoped <see cref="ITokenStore"/> service.
        /// </summary>
        /// <typeparam name="T">
        ///   The <see cref="ITokenStore"/> implementation type.
        /// </typeparam>
        /// <param name="builder">
        ///   The <see cref="IIndustrialAppStoreBuilder"/>.
        /// </param>
        /// <param name="implementationFactory">
        ///   The factory that will create the <typeparamref name="T"/> instances. The <see cref="HttpClient"/> 
        ///   parameter passed to the factory is an HTTP client that can be used to communicate 
        ///   with the Industrial App Store token endpoint.
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
        ///   The <see cref="HttpClient"/> parameter passed to the <paramref name="implementationFactory"/> 
        ///   delegate is an HTTP client that can be used to communicate with the Industrial App 
        ///   Store token endpoint.
        /// </remarks>
        public static IIndustrialAppStoreBuilder AddTokenStore<T>(this IIndustrialAppStoreBuilder builder, Func<IServiceProvider, HttpClient, T> implementationFactory) where T : class, ITokenStore {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(implementationFactory);

            builder.Services.TryAddEnumerable(ServiceDescriptor.Scoped<ITokenStore, T>(provider => {
                var http = provider.GetRequiredService<IHttpClientFactory>().CreateClient("ias_oauth_backchannel");
                return implementationFactory.Invoke(provider, http);
            }));

            return builder;
        }


        /// <summary>
        /// Registers Industrial App Store authentication schemes. 
        /// </summary>
        /// <param name="builder">
        ///   The <see cref="IIndustrialAppStoreBuilder"/>.
        /// </param>
        /// <param name="configure">
        ///   A delegate that can be used to configure the authentication options.
        /// </param>
        /// <returns>
        ///   The <see cref="IIndustrialAppStoreBuilder"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="builder"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="configure"/> is <see langword="null"/>.
        /// </exception>
        public static IIndustrialAppStoreBuilder AddAuthentication(this IIndustrialAppStoreBuilder builder, Action<IndustrialAppStoreAuthenticationOptions> configure) {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(configure);

            // Add authentication options.
            builder.Services.AddOptions<IndustrialAppStoreAuthenticationOptions>().Configure(configure);

            // Register post-configure actions for IndustrialAppStoreHttpClientOptions and
            // AuthenticationOptions if they are not already registered.
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<IndustrialAppStoreHttpClientOptions>, PostConfigureIasClientOptions>());
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<AuthenticationOptions>, PostConfigureAuthenticationOptions>());

            // Add authentication schemes.
            builder.Services.AddAuthentication()
                .AddIndustrialAppStoreSessionCookie()
                .AddIndustrialAppStoreOAuth();

            return builder;
        }


        /// <summary>
        /// Configures cookie authentication for the application.
        /// </summary>
        /// <param name="builder">
        ///   The authentication builder.
        /// </param>
        /// <returns>
        ///   The authentication builder.
        /// </returns>
        private static AuthenticationBuilder AddIndustrialAppStoreSessionCookie(this AuthenticationBuilder builder) {
            // Register a post-configure action for the cookie options if it is not already registered.
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<CookieAuthenticationOptions>, PostConfigureIasCookieAuthenticationOptions>());
            return builder.AddCookie();
        }


        /// <summary>
        /// Configures OAuth authentication for the application.
        /// </summary>
        /// <param name="builder">
        ///   The authentication builder.
        /// </param>
        /// <returns>
        ///   The authentication builder.
        /// </returns>
        private static AuthenticationBuilder AddIndustrialAppStoreOAuth(this AuthenticationBuilder builder) {
            // Register a post-configure action for the OAuth options if it is not already registered.
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<OAuthOptions>, PostConfigureIasOAuthOptions>());
            return builder.AddOAuth(IndustrialAppStoreAuthenticationDefaults.AuthenticationScheme,
                IndustrialAppStoreAuthenticationDefaults.DisplayName,
                options => { });
        }

    }
}
