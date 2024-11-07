using IntelligentPlant.IndustrialAppStore.Client;

using Microsoft.Extensions.Options;

namespace IntelligentPlant.IndustrialAppStore.Authentication.Options {

    /// <summary>
    /// <see cref="IPostConfigureOptions{TOptions}"/> that configures the base API endpoints on the
    /// registered <see cref="IndustrialAppStoreHttpClientOptions"/>.
    /// </summary>
    internal class PostConfigureIasClientOptions : IPostConfigureOptions<IndustrialAppStoreHttpClientOptions> {

        private readonly IOptionsMonitor<IndustrialAppStoreAuthenticationOptions> _options;


        public PostConfigureIasClientOptions(IOptionsMonitor<IndustrialAppStoreAuthenticationOptions> options) {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }


        public void PostConfigure(string? name, IndustrialAppStoreHttpClientOptions options) {
            var iasAuthOptions = _options.CurrentValue;

            if (!string.IsNullOrWhiteSpace(iasAuthOptions.DataCoreUrl)) {
                options.DataCoreUrl = new Uri(iasAuthOptions.DataCoreUrl.EndsWith('/') ? iasAuthOptions.DataCoreUrl : iasAuthOptions.DataCoreUrl + '/', UriKind.Absolute);
            }
            if (!string.IsNullOrWhiteSpace(iasAuthOptions.IndustrialAppStoreUrl)) {
                options.IndustrialAppStoreUrl = new Uri(iasAuthOptions.IndustrialAppStoreUrl.EndsWith('/') ? iasAuthOptions.IndustrialAppStoreUrl : iasAuthOptions.IndustrialAppStoreUrl + '/', UriKind.Absolute);
            }
        }
    }
}
