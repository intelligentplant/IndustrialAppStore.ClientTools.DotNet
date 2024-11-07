using IntelligentPlant.IndustrialAppStore.Client;

using Microsoft.Extensions.Options;

namespace IntelligentPlant.IndustrialAppStore.CommandLine.Options {

    /// <summary>
    /// <see cref="IPostConfigureOptions{TOptions}"/> that configures the base API endpoints on the
    /// registered <see cref="IndustrialAppStoreHttpClientOptions"/>.
    /// </summary>
    internal class PostConfigureIasClientOptions : IPostConfigureOptions<IndustrialAppStoreHttpClientOptions> {

        private readonly IOptionsMonitor<IndustrialAppStoreSessionManagerOptions> _options;


        public PostConfigureIasClientOptions(IOptionsMonitor<IndustrialAppStoreSessionManagerOptions> options) {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }


        public void PostConfigure(string? name, IndustrialAppStoreHttpClientOptions options) {
            var sessionManagerOptions = _options.CurrentValue;

            options.IndustrialAppStoreUrl = sessionManagerOptions.IndustrialAppStoreUrl;
            options.DataCoreUrl = sessionManagerOptions.DataCoreUrl;
        }
    }
}
