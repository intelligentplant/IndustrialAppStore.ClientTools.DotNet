using System;
using System.Collections.Generic;

using IntelligentPlant.IndustrialAppStore.Client;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace IntelligentPlant.IndustrialAppStore.Authentication {

    /// <summary>
    /// Options for Industrial App Store authentication.
    /// </summary>
    public class IndustrialAppStoreAuthenticationOptions {

        /// <summary>
        /// The base URL for the Industrial App Store.
        /// </summary>
        public string IndustrialAppStoreUrl { get; set; } = IndustrialAppStoreHttpClientDefaults.AppStoreUrl;

        /// <summary>
        /// The base URL for the Data Core.
        /// </summary>
        public string DataCoreUrl { get; set; } = IndustrialAppStoreHttpClientDefaults.DataCoreUrl;

        /// <summary>
        /// When <see langword="true"/>, the Industrial App Store authentication handler will not be 
        /// added to the application. Other associated services (such as <see cref="IndustrialAppStoreHttpClient"/>) 
        /// will still be added.
        /// </summary>
        /// <remarks>
        /// 
        /// <para>
        ///   Set this property to <see langword="true"/> when you want to authenticate access to 
        ///   your application using an authentication provider other than the Industrial App 
        ///   Store. 
        /// </para>
        /// 
        /// <para>
        ///   In the majority of cases, you should not modify the value of this property. However, 
        ///   if you are writing an app that can run against either the Industrial App Store or an 
        ///   on-premises Data Core API endpoint, you should set this property to <see langword="true"/> 
        ///   when you are running against a local Data Core API endpoint.
        /// </para>
        /// 
        /// <para>
        ///   When running against an on-premises Data Core API endpoint, it is the responsibility 
        ///   of the hosting application to provide an authentication mechanism.
        /// </para>
        /// 
        /// </remarks>
        public bool UseExternalAuthentication { get; set; }

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

        /// <summary>
        /// A callback that can be used to customise the <see cref="IHttpClientBuilder"/> 
        /// registration used by the <see cref="IndustrialAppStoreHttpClient"/> application 
        /// service.
        /// </summary>
        public Action<IHttpClientBuilder> ConfigureHttpClient { get; set; }

        /// <summary>
        /// Additional event handlers for cookie authentication events raised by the application.
        /// </summary>
        /// <remarks>
        ///   Ignored when <see cref="UseExternalAuthentication"/> is <see langword="true"/>.
        /// </remarks>
        public CookieAuthenticationEvents CookieAuthenticationEvents { get; set; }

        /// <summary>
        /// Additional event handlers for OAuth authentication events raised when signing a user 
        /// into the application using the Industrial App Store.
        /// </summary>
        /// <remarks>
        ///   Ignored when <see cref="UseExternalAuthentication"/> is <see langword="true"/>.
        /// </remarks>
        public OAuthEvents OAuthEvents { get; set; }

    }
}
