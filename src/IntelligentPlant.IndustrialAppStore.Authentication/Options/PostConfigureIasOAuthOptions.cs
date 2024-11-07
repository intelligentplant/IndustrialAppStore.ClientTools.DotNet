using System.Net.Http.Headers;
using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace IntelligentPlant.IndustrialAppStore.Authentication.Options {

    /// <summary>
    /// <see cref="IPostConfigureOptions{TOptions}"/> that configures the <see cref="OAuthOptions"/> 
    /// for Industrial App Store authentication.
    /// </summary>
    internal class PostConfigureIasOAuthOptions : IPostConfigureOptions<OAuthOptions> {

        private readonly IOptionsMonitor<IndustrialAppStoreAuthenticationOptions> _iasOptions;

        public PostConfigureIasOAuthOptions(IOptionsMonitor<IndustrialAppStoreAuthenticationOptions> iasOptions) {
            _iasOptions = iasOptions ?? throw new ArgumentNullException(nameof(iasOptions));
        }


        public void PostConfigure(string? name, OAuthOptions options) {
            if (!IndustrialAppStoreAuthenticationDefaults.AuthenticationScheme.Equals(name, StringComparison.OrdinalIgnoreCase)) {
                return;
            }

            var iasAuthOptions = _iasOptions.CurrentValue;
            if (iasAuthOptions.UseExternalAuthentication) {
                return;
            }

            var baseUrl = iasAuthOptions.IndustrialAppStoreUrl;
            baseUrl = baseUrl?.TrimEnd('/');

            options.AuthorizationEndpoint = iasAuthOptions.GetAuthorizationEndpoint();
            options.TokenEndpoint = iasAuthOptions.GetTokenEndpoint();
            options.UserInformationEndpoint = iasAuthOptions.GetUserInformationEndpoint();
            options.CallbackPath = iasAuthOptions.CallbackPath;

            options.ClientId = iasAuthOptions.ClientId;
            options.ClientSecret = iasAuthOptions.ClientSecret!;

            options.UsePkce = true;

            // A client secret must always be specified in the OAuth options, but it is possible that 
            // the app has not been issued with a client secret and is using PKCE, so it is still able 
            // to use the authorization code flow. In these circumstances, we will set a default 
            // client secret in the OAuth options so that the options pass validation.
            if (string.IsNullOrWhiteSpace(options.ClientSecret)) {
                options.ClientSecret = IndustrialAppStoreAuthenticationExtensions.DefaultClientSecret;
            }

            foreach (var scope in iasAuthOptions.Scope) {
                options.Scope.Add(scope);
            }

            // Map claims to fields on the JSON user info object.
            options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "name");
            options.ClaimActions.MapJsonKey(ClaimTypes.Name, "displayName");
            options.ClaimActions.MapJsonKey(IndustrialAppStoreAuthenticationDefaults.OrgIdentifierClaimType, "orgId");
            options.ClaimActions.MapJsonKey(IndustrialAppStoreAuthenticationDefaults.OrgNameClaimType, "org");
            options.ClaimActions.MapJsonKey(IndustrialAppStoreAuthenticationDefaults.PictureClaimType, "picUrl");

            options.Events = new OAuthEvents() {
                OnAccessDenied = async context => {
                    if (iasAuthOptions.OAuthEvents?.OnAccessDenied != null) {
                        await iasAuthOptions.OAuthEvents.OnAccessDenied(context);
                    }
                },
                OnCreatingTicket = async context => {
                    if (iasAuthOptions.OAuthEvents?.OnCreatingTicket != null) {
                        await iasAuthOptions.OAuthEvents.OnCreatingTicket(context);
                    }

                    // Get user info from the Industrial App Store API.
                    var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                    var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                    response.EnsureSuccessStatusCode();

                    var user = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(
                        await response.Content.ReadAsStringAsync()
                    );

                    context.RunClaimActions(user);

                    // Add session ID claim to identity. This can be used by custom
                    // ITokenStore implementations to distinguish between different login
                    // sessions from the same account.
                    var sessionId = iasAuthOptions.SessionIdGenerator?.Invoke(context.Identity!, context.HttpContext);
                    if (string.IsNullOrWhiteSpace(sessionId)) {
                        sessionId = Guid.NewGuid().ToString("N");
                    }
                    context.Identity!.AddClaim(new Claim(IndustrialAppStoreAuthenticationDefaults.AppSessionIdClaimType, sessionId));

                    var tokens = TokenStore.CreateOAuthTokens(
                        context.TokenType!,
                        context.AccessToken!,
                        context.ExpiresIn,
                        context.RefreshToken!,
                        context.HttpContext.RequestServices.GetRequiredService<TimeProvider>()
                    );

                    context.HttpContext.Items["ias_tokens"] = tokens;
                },
                OnRedirectToAuthorizationEndpoint = async context => {
                    if (iasAuthOptions.OAuthEvents?.OnRedirectToAuthorizationEndpoint != null) {
                        await iasAuthOptions.OAuthEvents.OnRedirectToAuthorizationEndpoint(context);
                    }

                    var queryParameters = new Dictionary<string, string?>();

                    if (iasAuthOptions.ShowConsentPrompt) {
                        queryParameters["prompt"] = "consent";
                    }
                    if (iasAuthOptions.RequestRefreshToken && (context.Properties.AllowRefresh ?? true)) {
                        // Request offline access if the options specify to request it, and 
                        // the authentication session does not explicitly prevent 
                        // refreshing of the session.
                        queryParameters["access_type"] = "offline";
                    }

                    if (queryParameters.Count > 0) {
                        context.RedirectUri = Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString(context.RedirectUri, queryParameters);
                    }

                    context.Response.Redirect(context.RedirectUri);
                },
                OnRemoteFailure = async context => {
                    if (iasAuthOptions.OAuthEvents?.OnRemoteFailure != null) {
                        await iasAuthOptions.OAuthEvents.OnRemoteFailure(context);
                    }
                },
                OnTicketReceived = async context => {
                    if (iasAuthOptions.OAuthEvents?.OnTicketReceived != null) {
                        await iasAuthOptions.OAuthEvents.OnTicketReceived(context);
                    }

                    if (!context.HttpContext.Items.TryGetValue("ias_tokens", out var o) || o == null) {
                        return;
                    }
                    var tokens = (OAuthTokens) o;

                    context.HttpContext.Items.Remove("ias_tokens");

                    var tokenStore = context.HttpContext.RequestServices.GetRequiredService<ITokenStore>();
                    await tokenStore.InitTokenStoreAsync(context.Principal!, context.Properties!);

                    await tokenStore.SaveTokensAsync(tokens);
                }
            };
        }

    }
}
