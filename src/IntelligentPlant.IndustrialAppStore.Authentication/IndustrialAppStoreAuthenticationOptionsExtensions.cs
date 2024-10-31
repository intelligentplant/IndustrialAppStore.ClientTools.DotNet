using System;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace IntelligentPlant.IndustrialAppStore.Authentication {

    /// <summary>
    /// Extensions for <see cref="IndustrialAppStoreAuthenticationOptions"/>.
    /// </summary>
    public static class IndustrialAppStoreAuthenticationOptionsExtensions {

        private static string GetEndpoint(this IndustrialAppStoreAuthenticationOptions options, string relativePath) {
            var baseUrl = options.IndustrialAppStoreUrl;
            baseUrl = baseUrl?.TrimEnd('/');
            return string.Concat(baseUrl, relativePath);
        }


        internal static string GetAuthorizationEndpoint(this IndustrialAppStoreAuthenticationOptions options) {
            return options.GetEndpoint("/AuthorizationServer/OAuth/Authorize");
        }


        internal static string GetTokenEndpoint(this IndustrialAppStoreAuthenticationOptions options) {
            return options.GetEndpoint("/AuthorizationServer/OAuth/Token");
        }


        internal static string GetUserInformationEndpoint(this IndustrialAppStoreAuthenticationOptions options) {
            return options.GetEndpoint("/api/user-search/me");
        }


        /// <summary>
        /// Gets or creates the session ID for the specified <see cref="HttpContext"/>.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="HttpContext"/>.
        /// </param>
        /// <param name="clientId">
        ///   The Industrial App Store client ID for the app.
        /// </param>
        /// <param name="configureCookie">
        ///   A callback that can be used to customise the <see cref="CookieOptions"/> for the 
        ///   device ID cookie.
        /// </param>
        /// <returns>
        ///   A session ID for the calling browser.
        /// </returns>
        /// <remarks>
        ///   The session ID is a GUID, which is encrypted using <see cref="IDataProtector"/> and 
        ///   saved in a persistent cookie in the browser. This allows the same session identifier 
        ///   to be re-used for the same browser/device combination across login sessions.
        /// </remarks>
        private static string GetOrCreateSessionId(HttpContext context, string clientId, Action<CookieOptions>? configureCookie) {
            const string cookieName = ".IndustrialAppStore.App.DeviceId";

            var provider = context.RequestServices.GetRequiredService<IDataProtectionProvider>();
            var dataProtector = provider.CreateProtector("IndustrialAppStore.Authentication", "deviceId", clientId);

            string sessionId;

            void CreateSessionId() {
                sessionId = Guid.NewGuid().ToString("N");

                var cookieOptions = new CookieOptions() {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Lax,
                    Secure = context.Request.IsHttps,
                    Expires = new DateTimeOffset(2038, 1, 1, 0, 0, 0, TimeSpan.Zero)
                };

                configureCookie?.Invoke(cookieOptions);

                context.Response.Cookies.Append(cookieName, dataProtector.Protect(sessionId), cookieOptions);
            }

            if (context.Request.Cookies.TryGetValue(cookieName, out var deviceId)) {
                try {
                    sessionId = dataProtector.Unprotect(deviceId!);
                }
                catch {
                    CreateSessionId();
                }
            }
            else {
                CreateSessionId();
            }

            return sessionId;
        }


        /// <summary>
        /// Updates the <see cref="IndustrialAppStoreAuthenticationOptions.SessionIdGenerator"/> 
        /// to derive a session ID from a device ID cookie in a user's browser, so that the same 
        /// session ID can be used every time the user logs in using that browser.
        /// </summary>
        /// <param name="options">
        ///   The <see cref="IndustrialAppStoreAuthenticationOptions"/>.
        /// </param>
        /// <param name="configure">
        ///   An optional callback that can be used to configure the properties of the device ID 
        ///   cookie when it is set.
        /// </param>
        /// <returns>
        ///   The <see cref="IndustrialAppStoreAuthenticationOptions"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="options"/> is <see langword="null"/>.
        /// </exception>
        /// <remarks>
        ///   Where an device ID cookie does not already exist, a random, globally-unique value 
        ///   will be generated and used. That is, the identifier is not derived from any 
        ///   properties of the calling user agent. The cookie value is always encrypted using 
        ///   ASP.NET Core's <see cref="IDataProtector"/> service.
        /// </remarks>
        public static IndustrialAppStoreAuthenticationOptions UseCookieSessionIdGenerator(this IndustrialAppStoreAuthenticationOptions options, Action<CookieOptions>? configure = null) {
            if (options == null) {
                throw new ArgumentNullException(nameof(options));
            }

            options.SessionIdGenerator = (identity, context) => GetOrCreateSessionId(context, options.ClientId, configure);

            return options;
        }

    }
}
