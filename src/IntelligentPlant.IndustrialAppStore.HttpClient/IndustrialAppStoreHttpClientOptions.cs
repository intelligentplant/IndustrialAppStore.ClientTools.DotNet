using System;
using IntelligentPlant.DataCore.Client;

namespace IntelligentPlant.IndustrialAppStore.Client {

    /// <summary>
    /// Options for <see cref="IndustrialAppStoreHttpClient"/>.
    /// </summary>
    public class IndustrialAppStoreHttpClientOptions : DataCoreHttpClientOptions {

        /// <summary>
        /// The base Industrial App Store API URL.
        /// </summary>
        public Uri IndustrialAppStoreUrl { get; set; }


        /// <summary>
        /// Creates a new <see cref="IndustrialAppStoreHttpClientOptions"/> object.
        /// </summary>
        public IndustrialAppStoreHttpClientOptions() {
            IndustrialAppStoreUrl = new Uri(IndustrialAppStoreHttpClientDefaults.AppStoreUrl);
            DataCoreUrl = new Uri(IndustrialAppStoreHttpClientDefaults.DataCoreUrl);
        }

    }
}
