using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

using IntelligentPlant.DataCore.Client;
using IntelligentPlant.IndustrialAppStore.Authentication;
using IntelligentPlant.IndustrialAppStore.Client;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;

namespace Microsoft.Extensions.DependencyInjection {

    /// <summary>
    /// Authentication extensions for the Intelligent Plant Industrial App Store.
    /// </summary>
    public static class IndustrialAppStoreAuthenticationExtensions {

#if NETCOREAPP3_1_OR_GREATER
        /// <summary>
        /// A client secret must always be specified in the OAuth options, but it is possible that 
        /// the app has not been issued with a client secret and is using PKCE, so it is still able 
        /// to use the authorization code flow. In these circumstances, we will set a default 
        /// client secret in the OAuth options so that the options pass validation.
        /// </summary>
        internal const string DefaultClientSecret = "IndustrialAppStore";
#endif

        /// <summary>
        /// Adds Industrial App Store authentication and required services to the application. 
        /// Register your app at https://appstore.intelligentplant.com!
        /// </summary>
        /// <param name="services">
        ///   The <see cref="IServiceCollection"/>.
        /// </param>
        /// <param name="configure">
        ///   A callback that is used to configure the authentication options.
        /// </param>
        /// <returns>
        ///   The <see cref="IServiceCollection"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="services"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="configure"/> is <see langword="null"/>.
        /// </exception>
        public static IServiceCollection AddIndustrialAppStoreAuthentication(
            this IServiceCollection services,
            Action<IndustrialAppStoreAuthenticationOptions> configure
        ) {
            if (services == null) {
                throw new ArgumentNullException(nameof(services));
            }
            if (configure == null) {
                throw new ArgumentNullException(nameof(configure));
            }

            // Configure options.
            var opts = new IndustrialAppStoreAuthenticationOptions();
            configure.Invoke(opts);

            services.AddSingleton(opts);
            services.AddSingleton(new IndustrialAppStoreHttpClientOptions() { 
                IndustrialAppStoreUrl = new Uri(opts.IndustrialAppStoreUrl),
                DataCoreUrl = new Uri(opts.DataCoreUrl)
            });

            services.AddHttpContextAccessor();
            services.AddHttpClient<ITokenStore, DefaultTokenStore>();

            var httpBuilder = services.AddHttpClient<IndustrialAppStoreHttpClient>(options => {
                options.BaseAddress = new Uri(opts.DataCoreUrl);
            });

            if (opts.UseExternalAuthentication) {
                services.AddAuthentication();
                httpBuilder.ConfigurePrimaryHttpMessageHandler(() => {
                    var result = new HttpClientHandler() {
                        Credentials = System.Net.CredentialCache.DefaultNetworkCredentials
                    };

                    return result;
                });
            } 
            else {
                httpBuilder.AddHttpMessageHandler(() => {
                    return DataCoreHttpClient.CreateAuthenticationMessageHandler<HttpContext>(
                        async (req, ctx, ct) => {
                            if (ctx == null) {
                                return null;
                            }
                            return await ctx
                                .RequestServices
                                .GetRequiredService<ITokenStore>()
                                .GetAuthenticationHeaderAsync();
                        }
                    );
                });

                services.AddAuthentication(options => {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                    if (PathString.Empty.Equals(opts.LoginPath)) {
                        // No login path specified; challenges will be issued automatically by IAS authentication.
                        options.DefaultChallengeScheme = IndustrialAppStoreAuthenticationDefaults.AuthenticationScheme;
                    }
                    else {
                        // Login path specified; challenges will be issued externally.
                        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    }
                })
                .AddCookie(options => {
                    if (!PathString.Empty.Equals(opts.LoginPath)) {
                        options.LoginPath = opts.LoginPath;
                    }

                    options.Events = new CookieAuthenticationEvents() {
                        OnValidatePrincipal = async ctx => {
                            var httpClientFactory = ctx.HttpContext.RequestServices.GetRequiredService<IHttpClientFactory>();
                            var httpClient = httpClientFactory.CreateClient(nameof(DefaultTokenStore));
                            var accessToken = await ctx.Properties.GetAccessTokenAsync(
                                ctx.HttpContext.RequestServices.GetRequiredService<IndustrialAppStoreAuthenticationOptions>(),
                                httpClient,
                                ctx.HttpContext.RequestServices.GetRequiredService<ISystemClock>(),
                                ctx.HttpContext.RequestAborted
                            );

                            if (string.IsNullOrWhiteSpace(accessToken)) {
                            // We do not have a valid access token for the calling user, so we 
                            // will consider the cookie to be invalid.
                            ctx.RejectPrincipal();
                            }
                        }
                    };
                })
                .AddIndustrialAppStoreOAuthAuthentication(opts);
            }

            opts.ConfigureHttpClient?.Invoke(httpBuilder);

            return services;
        }


        /// <summary>
        /// Adds Industrial App Store authentication to the application. Register your app at 
        /// https://appstore.intelligentplant.com!
        /// </summary>
        /// <param name="builder">
        ///   The authentication builder.
        /// </param>
        /// <param name="opts">
        ///   The authentication options.
        /// </param>
        /// <returns>
        ///   The authentication builder.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="builder"/> is <see langword="null"/>.
        /// </exception>
        private static AuthenticationBuilder AddIndustrialAppStoreOAuthAuthentication(
            this AuthenticationBuilder builder,
            IndustrialAppStoreAuthenticationOptions opts
        ) {
            if (builder == null) {
                throw new ArgumentNullException(nameof(builder));
            }

            // Register IAS OAuth.
            builder.AddOAuth(
                IndustrialAppStoreAuthenticationDefaults.AuthenticationScheme, 
                IndustrialAppStoreAuthenticationDefaults.DisplayName,
                options => {
                    var baseUrl = opts.IndustrialAppStoreUrl;
                    baseUrl = baseUrl?.TrimEnd('/');

                    options.AuthorizationEndpoint = opts.GetAuthorizationEndpoint();
                    options.TokenEndpoint = opts.GetTokenEndpoint();
                    options.UserInformationEndpoint = opts.GetUserInformationEndpoint();
                    options.CallbackPath = opts.CallbackPath;

                    options.ClientId = opts.ClientId;
                    options.ClientSecret = opts.ClientSecret;
#if NETCOREAPP3_1_OR_GREATER
                    options.UsePkce = true;
                    // Microsoft OAuth authentication middleware requires a client secret to be 
                    // specified, even when PKCE is being used. This can be any non-empty value; 
                    // it just has to be set.
                    if (string.IsNullOrWhiteSpace(options.ClientSecret)) {
                        options.ClientSecret = DefaultClientSecret;
                    }
#endif

                    foreach (var scope in opts.Scope) {
                        options.Scope.Add(scope);
                    }

                    options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "name");
                    options.ClaimActions.MapJsonKey(ClaimTypes.Name, "displayName");
                    options.ClaimActions.MapJsonKey(IndustrialAppStoreAuthenticationDefaults.OrgIdentifierClaimType, "orgId");
                    options.ClaimActions.MapJsonKey(IndustrialAppStoreAuthenticationDefaults.OrgNameClaimType, "org");
                    options.ClaimActions.MapJsonKey(IndustrialAppStoreAuthenticationDefaults.PictureClaimType, "picUrl");

                    // Save the tokens into authentication ticket properties.
                    options.SaveTokens = true;

                    options.Events = new OAuthEvents() {
                        OnRedirectToAuthorizationEndpoint = context => {
                            var queryParameters = new Dictionary<string, string>();

                            if (opts.ShowConsentPrompt) {
                                queryParameters["prompt"] = "consent";
                            }
                            if (opts.RequestRefreshToken && (context.Properties.AllowRefresh ?? true)) {
                                // Request offline access if the options specify to request it, and 
                                // the authentication session does not explicitly prevent 
                                // refreshing of the session.
                                queryParameters["access_type"] = "offline";
                            }

                            if (queryParameters.Count > 0) {
                                context.RedirectUri = AspNetCore.WebUtilities.QueryHelpers.AddQueryString(context.RedirectUri, queryParameters);
                            }

                            context.Response.Redirect(context.RedirectUri);
                            return Task.CompletedTask;
                        },
                        OnCreatingTicket = async context => {
                            var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                            var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                            response.EnsureSuccessStatusCode();

#if NETCOREAPP3_1_OR_GREATER
                            var user = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(
                                await response.Content.ReadAsStringAsync()
                            );
#else
                            var user = Newtonsoft.Json.Linq.JObject.Parse(await response.Content.ReadAsStringAsync());
#endif

                            context.RunClaimActions(user);
                        }
                    };
                }
            );

            return builder;
        }

    }
}
