﻿using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

using IntelligentPlant.IndustrialAppStore.Client;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;

namespace IntelligentPlant.IndustrialAppStore.Authentication {

    /// <summary>
    /// Options for Industrial App Store authentication.
    /// </summary>
    public class IndustrialAppStoreAuthenticationOptions {

        /// <summary>
        /// The base URL for the Industrial App Store.
        /// </summary>
        [Required]
        public string IndustrialAppStoreUrl { get; set; } = IndustrialAppStoreHttpClientDefaults.AppStoreUrl;

        /// <summary>
        /// The base URL for the Data Core.
        /// </summary>
        [Required]
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
        [Required]
        public string ClientId { get; set; } = default!;

        /// <summary>
        /// The Industrial App Store client secret.
        /// </summary>
        public string? ClientSecret { get; set; }

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
        /// The expiry time for the cookie used to store the user's session.
        /// </summary>
        /// <remarks>
        ///  If <see langword="null"/>, the cookie will use the default expiry period for an 
        ///  ASP.NET Core authentication cookie.
        /// </remarks>
        public TimeSpan? CookieExpiry { get; set; }

        /// <summary>
        /// Additional event handlers for cookie authentication events raised by the application.
        /// </summary>
        /// <remarks>
        ///   Ignored when <see cref="UseExternalAuthentication"/> is <see langword="true"/>.
        /// </remarks>
        public CookieAuthenticationEvents? CookieAuthenticationEvents { get; set; }

        /// <summary>
        /// Additional event handlers for OAuth authentication events raised when signing a user 
        /// into the application using the Industrial App Store.
        /// </summary>
        /// <remarks>
        ///   Ignored when <see cref="UseExternalAuthentication"/> is <see langword="true"/>.
        /// </remarks>
        public OAuthEvents? OAuthEvents { get; set; }

        /// <summary>
        /// A callback that can be used to generate an ID for a new session from the provided 
        /// <see cref="ClaimsIdentity"/> and <see cref="HttpContext"/>.
        /// </summary>
        /// <remarks>
        /// 
        /// <para>
        ///   Specify a value for <see cref="SessionIdGenerator"/> if your app nees to customise 
        ///   how an identifier is generated for a session (for example, by computing a fingerprint 
        ///   based on the user/device/browser combination).
        /// </para>
        /// 
        /// <para>
        ///   The <seealso cref="IndustrialAppStoreAuthenticationOptionsExtensions.UseCookieSessionIdGenerator"/> 
        ///   extension method can be used to assign a value to <see cref="SessionIdGenerator"/> 
        ///   that will store an identifier for the browser in a persistent cookie and re-use it 
        ///   across different login sessions.
        /// </para>
        /// 
        /// <para>
        ///   If <see cref="SessionIdGenerator"/> is <see langword="null"/> or returns a 
        ///   <see langword="null"/> or white space value, a unique identifier will be generated 
        ///   for the session.
        /// </para>
        /// 
        /// </remarks>
        /// <seealso cref="IndustrialAppStoreAuthenticationOptionsExtensions.UseCookieSessionIdGenerator"/>
        public Func<ClaimsIdentity, HttpContext, string>? SessionIdGenerator { get; set; }

    }
}
