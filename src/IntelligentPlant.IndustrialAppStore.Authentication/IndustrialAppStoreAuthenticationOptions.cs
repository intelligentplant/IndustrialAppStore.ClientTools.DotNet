using System.Collections.Generic;
using IntelligentPlant.IndustrialAppStore.Client;
using Microsoft.AspNetCore.Http;

namespace IntelligentPlant.IndustrialAppStore.Authentication {

    /// <summary>
    /// Options for Industrial App Store authentication.
    /// </summary>
    public class IndustrialAppStoreAuthenticationOptions {

        /// <summary>
        /// The base URL for the Industrial App Store.
        /// </summary>
        public string AppStoreUrl { get; set; } = IndustrialAppStoreHttpClientDefaults.AppStoreUrl;

        /// <summary>
        /// The base URL for the Data Core.
        /// </summary>
        public string DataCoreUrl { get; set; } = IndustrialAppStoreHttpClientDefaults.DataCoreUrl;

        /// <summary>
        /// The Industrial App Store client ID.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// The Industrial App Store client secret.
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// The OAuth scopes to request.
        /// </summary>
        public ICollection<string> Scope { get; } = new List<string>();

        /// <summary>
        /// The login path for the application. When <see cref="PathString.Empty"/> is specified, 
        /// unauthenticated users will be redirected to the Industrial App Store for sign-in as 
        /// soon as they attempt to access an authenticated route.
        /// </summary>
        public PathString LoginPath { get; set; } = PathString.Empty;

        /// <summary>
        /// The OAuth callback path to use.
        /// </summary>
        public PathString CallbackPath { get; set; } = IndustrialAppStoreAuthenticationDefaults.DefaultCallbackPath;

        /// <summary>
        /// When <see langword="true"/>, the app will request a refresh token when signing a user 
        /// in.
        /// </summary>
        public bool RequestRefreshToken { get; set; }

        /// <summary>
        /// When <see langword="true"/>, the Industrial App Store consent screen will always be 
        /// displayed when logging a user in. The consent screen is always displayed the first 
        /// time the user signs into the app, or if the scopes requested by the app have changed 
        /// since the last time the user signed in.
        /// </summary>
        public bool ShowConsentPrompt { get; set; }

    }
}
