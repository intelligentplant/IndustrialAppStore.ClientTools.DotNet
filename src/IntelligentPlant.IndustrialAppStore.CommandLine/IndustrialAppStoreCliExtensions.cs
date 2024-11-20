using IntelligentPlant.IndustrialAppStore.Client;
using IntelligentPlant.IndustrialAppStore.CommandLine;
using IntelligentPlant.IndustrialAppStore.CommandLine.Options;
using IntelligentPlant.IndustrialAppStore.DependencyInjection;

using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection {

    /// <summary>
    /// Extensions for registering Industrial App Store services for CLI applications.
    /// </summary>
    public static class IndustrialAppStoreCliExtensions {

        /// <summary>
        /// Registers services required for calling the Industrial App Store API in a CLI application.
        /// </summary>
        /// <param name="services">
        ///   The service collection.
        /// </param>
        /// <param name="configure">
        ///   A delegate that is used to configure the options for the <see cref="IndustrialAppStoreSessionManager"/> service.
        /// </param>
        /// <returns>
        ///   An <see cref="IIndustrialAppStoreBuilder"/> that can be used to configure additional 
        ///   Industrial App Store services.
        /// </returns>
        /// <remarks>
        /// 
        /// <para>
        ///   <see cref="IndustrialAppStoreSessionManager"/> is a singleton service that manages 
        ///   the Industrial App Store authentication for the applicaiton. To ensure that the user 
        ///   has signed the application into the Industrial App Store, call the <see cref="IndustrialAppStoreSessionManager.SignInAsync"/> 
        ///   method.
        /// </para>
        /// 
        /// <para>
        ///   Use the scoped <see cref="IndustrialAppStoreHttpClient"/> service to obtain an API 
        ///   client that automatically authenticates API calls using the authentication session 
        ///   held by the <see cref="IndustrialAppStoreSessionManager"/> service.
        /// </para>
        /// 
        /// </remarks>
        public static IIndustrialAppStoreBuilder AddIndustrialAppStoreCliServices(this IServiceCollection services, Action<IndustrialAppStoreSessionManagerOptions> configure) {
            var builder = services.AddIndustrialAppStoreApiServices()
                .AddAccessTokenProvider(provider => {
                    var sessionManager = provider.GetRequiredService<IndustrialAppStoreSessionManager>();
                    return ct => sessionManager.GetAccessTokenAsync(ct);
                });

            builder.Services.AddSingleton(TimeProvider.System);

            builder.Services.AddDataProtection();

            builder.Services.AddHttpClient(nameof(IndustrialAppStoreSessionManager));

            builder.Services.AddOptions<IndustrialAppStoreSessionManagerOptions>()
                .Configure(configure)
                .ValidateDataAnnotations();

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<IndustrialAppStoreHttpClientOptions>, PostConfigureIasClientOptions>());

            builder.Services.AddSingleton(provider => {
                var options = provider.GetRequiredService<IOptions<IndustrialAppStoreSessionManagerOptions>>().Value;
                return new AppDataFolderProvider(options.AppDataPath);
            });

            builder.Services.AddSingleton(provider => {
                var options = provider.GetRequiredService<IOptions<IndustrialAppStoreSessionManagerOptions>>().Value;
                return ActivatorUtilities.CreateInstance<IndustrialAppStoreSessionManager>(provider, options);
            });

            return builder;
        }

    }

}
