using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace IntelligentPlant.IndustrialAppStore.Authentication.Options {

    /// <summary>
    /// <see cref="IPostConfigureOptions{TOptions}"/> that configures authentication event 
    /// handlers and other properties on the registered <see cref="CookieAuthenticationOptions"/>.
    /// </summary>
    internal class PostConfigureIasCookieAuthenticationOptions : IPostConfigureOptions<CookieAuthenticationOptions> {

        private readonly IOptionsMonitor<IndustrialAppStoreAuthenticationOptions> _iasOptions;


        public PostConfigureIasCookieAuthenticationOptions(IOptionsMonitor<IndustrialAppStoreAuthenticationOptions> iasOptions) {
            _iasOptions = iasOptions ?? throw new ArgumentNullException(nameof(iasOptions));
        }


        public void PostConfigure(string? name, CookieAuthenticationOptions options) {
            var iasOptions = _iasOptions.CurrentValue;

            if (iasOptions.UseExternalAuthentication) {
                return;
            }

            if (!PathString.Empty.Equals(iasOptions.LoginPath)) {
                options.LoginPath = iasOptions.LoginPath;
            }

            if (iasOptions.CookieExpiry.HasValue) {
                options.ExpireTimeSpan = iasOptions.CookieExpiry.Value;
            }

            var cookieEvents = iasOptions.CookieAuthenticationEvents ?? new CookieAuthenticationEvents();

            options.Events = new CookieAuthenticationEvents() {
                OnCheckSlidingExpiration = async ctx => {
                    if (cookieEvents.OnCheckSlidingExpiration != null) {
                        await cookieEvents.OnCheckSlidingExpiration(ctx).ConfigureAwait(false);
                    }
                },
                OnRedirectToAccessDenied = async ctx => {
                    if (cookieEvents.OnRedirectToAccessDenied != null) {
                        await cookieEvents.OnRedirectToAccessDenied(ctx).ConfigureAwait(false);
                    }
                },
                OnRedirectToLogin = async ctx => {
                    if (cookieEvents.OnRedirectToLogin != null) {
                        await cookieEvents.OnRedirectToLogin(ctx).ConfigureAwait(false);
                    }
                },
                OnRedirectToLogout = async ctx => {
                    if (cookieEvents.OnRedirectToLogout != null) {
                        await cookieEvents.OnRedirectToLogout(ctx).ConfigureAwait(false);
                    }
                },
                OnRedirectToReturnUrl = async ctx => {
                    if (cookieEvents.OnRedirectToReturnUrl != null) {
                        await cookieEvents.OnRedirectToReturnUrl(ctx).ConfigureAwait(false);
                    }
                },
                OnSignedIn = async ctx => {
                    if (cookieEvents.OnSignedIn != null) {
                        await cookieEvents.OnSignedIn(ctx).ConfigureAwait(false);
                    }
                },
                OnSigningIn = async ctx => {
                    if (cookieEvents.OnSigningIn != null) {
                        await cookieEvents.OnSigningIn(ctx).ConfigureAwait(false);
                    }
                },
                OnSigningOut = async ctx => {
                    if (cookieEvents.OnSigningOut != null) {
                        await cookieEvents.OnSigningOut(ctx).ConfigureAwait(false);
                    }
                },
                OnValidatePrincipal = async ctx => {
                    var tokenStore = ctx.HttpContext.RequestServices.GetRequiredService<ITokenStore>();
                    await tokenStore.InitTokenStoreAsync(ctx.Principal!, ctx.Properties).ConfigureAwait(false);

                    // The default cookie-based token store sets an authentication property
                    // specifying when the access token will expire. This will be null for
                    // other token stores.
                    var expiresAtOriginal = ctx.Properties.GetTokenValue(IndustrialAppStoreAuthenticationDefaults.ExpiresAtTokenName);

                    var accessToken = await tokenStore.GetTokensAsync().ConfigureAwait(false);

                    if (accessToken == null) {
                        // We do not have a valid access token for the calling user, so we 
                        // will consider the cookie to be invalid.
                        ctx.RejectPrincipal();
                    }

                    // If we are using the default cookie-based token store and the access
                    // token was refreshed by the GetTokensAsync call above, the authentiation
                    // property specifying the expires-at timestamp will have changed.
                    var expiresAtUpdated = ctx.Properties.GetTokenValue(IndustrialAppStoreAuthenticationDefaults.ExpiresAtTokenName);

                    // If the access token has been refreshed, we need to renew the authentication cookie.
                    ctx.ShouldRenew = !string.Equals(expiresAtOriginal, expiresAtUpdated, StringComparison.Ordinal);

                    if (cookieEvents.OnValidatePrincipal != null) {
                        await cookieEvents.OnValidatePrincipal(ctx).ConfigureAwait(false);
                    }
                }
            };
        }
    }
}
