using System;
using IntelligentPlant.DataCore.Client;

namespace IntelligentPlant.IndustrialAppStore.Client {

    /// <summary>
    /// Options for <see cref="IndustrialAppStoreHttpClient{TContext}"/>.
    /// </summary>
    public class IndustrialAppStoreHttpClientOptions : DataCoreHttpClientOptions {

        /// <summary>
        /// The base Industrial App Store URL.
        /// </summary>
        public Uri AppStoreUrl { get; set; }


        /// <summary>
        /// Creates a new <see cref="IndustrialAppStoreHttpClientOptions"/> object.
        /// </summary>
        public IndustrialAppStoreHttpClientOptions() {
            AppStoreUrl = new Uri(IndustrialAppStoreHttpClientDefaults.AppStoreUrl);
            DataCoreUrl = new Uri(IndustrialAppStoreHttpClientDefaults.DataCoreUrl);
        }

    }
}
