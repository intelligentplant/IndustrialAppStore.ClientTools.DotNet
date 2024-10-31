using System;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace IntelligentPlant.IndustrialAppStore.Authentication.Options {

    /// <summary>
    /// <see cref="IPostConfigureOptions{TOptions}"/> that configures default schemes on the 
    /// registered <see cref="AuthenticationOptions"/>.
    /// </summary>
    internal class PostConfigureAuthenticationOptions : IPostConfigureOptions<AuthenticationOptions> {

        private readonly IOptionsMonitor<IndustrialAppStoreAuthenticationOptions> _options;


        public PostConfigureAuthenticationOptions(IOptionsMonitor<IndustrialAppStoreAuthenticationOptions> options) {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }


        public void PostConfigure(string? name, AuthenticationOptions options) {
            var iasAuthOptions = _options.CurrentValue;

            if (iasAuthOptions.UseExternalAuthentication) {
                return;
            }

            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            if (PathString.Empty.Equals(iasAuthOptions.LoginPath)) {
                // No login path specified; challenges will be issued automatically by IAS OAuth authentication.
                options.DefaultChallengeScheme = IndustrialAppStoreAuthenticationDefaults.AuthenticationScheme;
            }
            else {
                // Login path specified; challenges will be issued by a login page.
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }
        }
    }
}
