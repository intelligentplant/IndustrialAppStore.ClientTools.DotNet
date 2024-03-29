﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

using IntelligentPlant.IndustrialAppStore.Authentication;
using IntelligentPlant.IndustrialAppStore.Client;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;

using HttpHandlerType = System.Net.Http.SocketsHttpHandler;

namespace Microsoft.Extensions.DependencyInjection {

    /// <summary>
    /// Authentication extensions for the Intelligent Plant Industrial App Store.
    /// </summary>
    public static class IndustrialAppStoreAuthenticationExtensions {

        /// <summary>
        /// A client secret must always be specified in the OAuth options, but it is possible that 
        /// the app has not been issued with a client secret and is using PKCE, so it is still able 
        /// to use the authorization code flow. In these circumstances, we will set a default 
        /// client secret in the OAuth options so that the options pass validation.
        /// </summary>
        internal const string DefaultClientSecret = "IndustrialAppStore";


        /// <summary>
        /// Adds Industrial App Store authentication and required services to the application, 
        /// using a default <see cref="ITokenStore"/> implementation that stores access tokens 
        /// in the application's session cookies. 
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
        /// <remarks>
        ///   Register your app <a href="https://appstore.intelligentplant.com">here</a>!
        /// </remarks>
        public static IServiceCollection AddIndustrialAppStoreAuthentication(
            this IServiceCollection services,
            Action<IndustrialAppStoreAuthenticationOptions> configure
        ) {
            return services.AddIndustrialAppStoreAuthentication<DefaultTokenStore>(configure);
        }


        /// <summary>
        /// Adds Industrial App Store authentication and required services to the application, 
        /// using a custom <see cref="ITokenStore"/> implementation to store and retrieve access 
        /// tokens for signed-in users. 
        /// </summary>
        /// <param name="services">
        ///   The <see cref="IServiceCollection"/>.
        /// </param>
        /// <param name="configure">
        ///   A callback that is used to configure the authentication options.
        /// </param>
        /// <param name="factory">
        ///   An optional factory method for creating an instance of <typeparamref name="TTokenStore"/>.
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
        /// <remarks>
        /// 
        /// <para>
        ///   Register your app <a href="https://appstore.intelligentplant.com">here</a>!
        /// </para>
        /// 
        /// <para>
        ///   If the <paramref name="configure"/> callback sets <see cref="IndustrialAppStoreAuthenticationOptions.UseExternalAuthentication"/> 
        ///   to <see langword="true"/>, no <see cref="ITokenStore"/> service will be registered 
        ///   with the <paramref name="services"/> collection.
        /// </para>
        /// 
        /// </remarks>
        /// <seealso cref="ITokenStore"/>
        /// <seealso cref="TokenStore"/>
        public static IServiceCollection AddIndustrialAppStoreAuthentication<TTokenStore>(
            this IServiceCollection services,
            Action<IndustrialAppStoreAuthenticationOptions> configure,
            Func<IServiceProvider, HttpClient, TTokenStore>? factory = null
        ) where TTokenStore : class, ITokenStore {
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
            services.AddSession();

            // HTTP client used by IndustrialAppStoreHttpClient.
            var userIasHttpClientBuilder = services.AddHttpClient<IndustrialAppStoreHttpClient>(options => {
                options.BaseAddress = new Uri(opts.DataCoreUrl);
            });

            // HTTP client used by BackchannelIndustrialAppStoreHttpClient.
            var backchannelIasHttpClientBuilder = services.AddHttpClient<BackchannelIndustrialAppStoreHttpClient>(options => {
                options.BaseAddress = new Uri(opts.DataCoreUrl);
            });

            // HTTP client for backchannel communications with the IAS OAuth token endpoint.
            services.AddHttpClient("ias_oauth_backchannel");

            services.AddScoped<ITokenStore, TTokenStore>(sp => {
                var http = sp.GetRequiredService<IHttpClientFactory>().CreateClient("ias_oauth_backchannel");
                return factory == null
                    ? ActivatorUtilities.CreateInstance<TTokenStore>(sp, http)
                    : factory.Invoke(sp, http);
            });

            if (opts.UseExternalAuthentication) {
                ConfigureExternalAuthentication(services, userIasHttpClientBuilder, backchannelIasHttpClientBuilder);
            } 
            else {
                ConfigureIndustrialAppStoreAuthentication(services, userIasHttpClientBuilder, backchannelIasHttpClientBuilder, opts, factory);
            }

            opts.ConfigureHttpClient?.Invoke(typeof(IndustrialAppStoreHttpClient), userIasHttpClientBuilder);
            opts.ConfigureHttpClient?.Invoke(typeof(BackchannelIndustrialAppStoreHttpClient), backchannelIasHttpClientBuilder);

            return services;
        }


        /// <summary>
        ///   Performs configuration related to non-Industrial App Store authentication.
        /// </summary>
        /// <param name="services">
        ///   The application service collection.
        /// </param>
        /// <param name="userHttpClientBuilder">
        ///   The <see cref="IHttpClientBuilder"/> for the <see cref="IndustrialAppStoreHttpClient"/> 
        ///   type.
        /// </param>
        /// <param name="backchannelHttpClientBuilder">
        ///   The <see cref="IHttpClientBuilder"/> for the <see cref="BackchannelIndustrialAppStoreHttpClient"/> 
        ///   type.
        /// </param>
        private static void ConfigureExternalAuthentication(IServiceCollection services, IHttpClientBuilder userHttpClientBuilder, IHttpClientBuilder backchannelHttpClientBuilder) {
            services.AddAuthentication();
            userHttpClientBuilder.ConfigurePrimaryHttpMessageHandler(() => {
                var result = new HttpHandlerType() {
                    Credentials = System.Net.CredentialCache.DefaultNetworkCredentials
                };

                return result;
            });
            backchannelHttpClientBuilder.ConfigurePrimaryHttpMessageHandler(() => {
                var result = new HttpHandlerType() {
                    Credentials = System.Net.CredentialCache.DefaultNetworkCredentials
                };

                return result;
            });
        }


        /// <summary>
        /// Configures Industrial App Store authentication for the application.
        /// </summary>
        /// <typeparam name="TTokenStore">
        ///   The <see cref="ITokenStore"/> implementation to use.
        /// </typeparam>
        /// <param name="services">
        ///   The application service collection.
        /// </param>
        /// <param name="userHttpClientBuilder">
        ///   The <see cref="IHttpClientBuilder"/> for the <see cref="IndustrialAppStoreHttpClient"/> 
        ///   type.
        /// </param>
        /// <param name="backchannelHttpClientBuilder">
        ///   The <see cref="IHttpClientBuilder"/> for the <see cref="BackchannelIndustrialAppStoreHttpClient"/> 
        ///   type.
        /// </param>
        /// <param name="iasOptions">
        ///   The <see cref="IndustrialAppStoreAuthenticationOptions"/> for the application.
        /// </param>
        /// <param name="factory">
        ///   An optional factory method for creating <typeparamref name="TTokenStore"/> instances.
        /// </param>
        private static void ConfigureIndustrialAppStoreAuthentication<TTokenStore>(
            IServiceCollection services, 
            IHttpClientBuilder userHttpClientBuilder,
            IHttpClientBuilder backchannelHttpClientBuilder,
            IndustrialAppStoreAuthenticationOptions iasOptions,
            Func<IServiceProvider, HttpClient, TTokenStore>? factory
        ) where TTokenStore : class, ITokenStore {
            userHttpClientBuilder.AddHttpMessageHandler(() => {
                return IndustrialAppStoreHttpClient.CreateAuthenticationMessageHandler(
                    async (req, ctx, ct) => {
                        if (ctx == null) {
                            return null;
                        }
                        return await ctx
                            .RequestServices
                            .GetRequiredService<ITokenStore>()
                            .GetAuthenticationHeaderAsync()
                            .ConfigureAwait(false);
                    }
                );
            });

            backchannelHttpClientBuilder.AddHttpMessageHandler(() => {
                return BackchannelIndustrialAppStoreHttpClient.CreateAuthenticationMessageHandler(
                    async (req, tokenStore, ct) => {
                        if (tokenStore == null) {
                            return null;
                        }
                        return await tokenStore.GetAuthenticationHeaderAsync().ConfigureAwait(false);
                    }
                );
            });

            services.AddAuthentication(authOptions => {
                authOptions.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                authOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                authOptions.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                if (PathString.Empty.Equals(iasOptions.LoginPath)) {
                    // No login path specified; challenges will be issued automatically by IAS authentication.
                    authOptions.DefaultChallengeScheme = IndustrialAppStoreAuthenticationDefaults.AuthenticationScheme;
                }
                else {
                    // Login path specified; challenges will be issued externally.
                    authOptions.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                }
            })
            .AddCookie(cookieOptions => {
                if (!PathString.Empty.Equals(iasOptions.LoginPath)) {
                    cookieOptions.LoginPath = iasOptions.LoginPath;
                }

                var cookieEvents = iasOptions.CookieAuthenticationEvents ?? new CookieAuthenticationEvents();

                cookieOptions.Events = new CookieAuthenticationEvents() {
#if NET6_0_OR_GREATER
                    OnCheckSlidingExpiration = async ctx => { 
                        if (cookieEvents.OnCheckSlidingExpiration != null) {
                            await cookieEvents.OnCheckSlidingExpiration(ctx).ConfigureAwait(false);
                        }
                    },
#endif
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
                        await InitTokenStoreAsync(tokenStore, ctx.Principal!, ctx.Properties).ConfigureAwait(false);

                        var accessToken = await tokenStore.GetTokensAsync().ConfigureAwait(false);

                        if (accessToken == null) {
                            // We do not have a valid access token for the calling user, so we 
                            // will consider the cookie to be invalid.
                            ctx.RejectPrincipal();
                        }

                        if (cookieEvents.OnValidatePrincipal != null) {
                            await cookieEvents.OnValidatePrincipal(ctx).ConfigureAwait(false);
                        }
                    }
                };
            })
            .AddIndustrialAppStoreOAuthAuthentication(iasOptions);
        }


        /// <summary>
        /// Adds Industrial App Store authentication to the application.
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
                    options.ClientSecret = opts.ClientSecret!;

                    options.UsePkce = true;
                    // Microsoft OAuth authentication middleware requires a client secret to be 
                    // specified, even when PKCE is being used. This can be any non-empty value; 
                    // it just has to be set.
                    if (string.IsNullOrWhiteSpace(options.ClientSecret)) {
                        options.ClientSecret = DefaultClientSecret;
                    }

                    foreach (var scope in opts.Scope) {
                        options.Scope.Add(scope);
                    }

                    options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "name");
                    options.ClaimActions.MapJsonKey(ClaimTypes.Name, "displayName");
                    options.ClaimActions.MapJsonKey(IndustrialAppStoreAuthenticationDefaults.OrgIdentifierClaimType, "orgId");
                    options.ClaimActions.MapJsonKey(IndustrialAppStoreAuthenticationDefaults.OrgNameClaimType, "org");
                    options.ClaimActions.MapJsonKey(IndustrialAppStoreAuthenticationDefaults.PictureClaimType, "picUrl");

                    options.Events = new OAuthEvents() {
                        OnAccessDenied = async context => {
                            if (opts.OAuthEvents?.OnAccessDenied != null) {
                                await opts.OAuthEvents.OnAccessDenied(context);
                            }
                        },
                        OnCreatingTicket = async context => {
                            if (opts.OAuthEvents?.OnCreatingTicket != null) {
                                await opts.OAuthEvents.OnCreatingTicket(context);
                            }

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
                            var sessionId = opts.SessionIdGenerator?.Invoke(context.Identity!, context.HttpContext);
                            if (string.IsNullOrWhiteSpace(sessionId)) {
                                sessionId = Guid.NewGuid().ToString("N");
                            }
                            context.Identity!.AddClaim(new Claim(IndustrialAppStoreAuthenticationDefaults.AppSessionIdClaimType, sessionId));

                            var tokens = TokenStore.CreateOAuthTokens(
                                context.TokenType!, 
                                context.AccessToken!, 
                                context.ExpiresIn, 
                                context.RefreshToken!,
                                context.HttpContext.RequestServices.GetRequiredService<ISystemClock>()
                            );

                            context.HttpContext.Items["ias_tokens"] = tokens;
                        },
                        OnRedirectToAuthorizationEndpoint = async context => {
                            if (opts.OAuthEvents?.OnRedirectToAuthorizationEndpoint != null) {
                                await opts.OAuthEvents.OnRedirectToAuthorizationEndpoint(context);
                            }

                            var queryParameters = new Dictionary<string, string?>();

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
                        },
                        OnRemoteFailure = async context => { 
                            if (opts.OAuthEvents?.OnRemoteFailure != null) {
                                await opts.OAuthEvents.OnRemoteFailure(context);
                            }
                        },
                        OnTicketReceived = async context => {
                            if (opts.OAuthEvents?.OnTicketReceived != null) {
                                await opts.OAuthEvents.OnTicketReceived(context);
                            }

                            if (!context.HttpContext.Items.TryGetValue("ias_tokens", out var o) || o == null) {
                                return;
                            }
                            var tokens = (OAuthTokens) o;

                            context.HttpContext.Items.Remove("ias_tokens");

                            var tokenStore = context.HttpContext.RequestServices.GetRequiredService<ITokenStore>();
                            await InitTokenStoreAsync(tokenStore, context.Principal!, context.Properties!);

                            await tokenStore.SaveTokensAsync(tokens);
                        }
                    };
                }
            );

            return builder;
        }


        /// <summary>
        /// Initialises the token store using the specified principal and authentication properties.
        /// </summary>
        /// <param name="tokenStore">
        ///   The token store.
        /// </param>
        /// <param name="principal">
        ///   The principal to retrieve the user ID and session ID from.
        /// </param>
        /// <param name="properties">
        ///   The authentication properties.
        /// </param>
        /// <returns>
        ///   A <see cref="ValueTask"/> that will initialise the token store.
        /// </returns>
        private static async ValueTask InitTokenStoreAsync(ITokenStore tokenStore, ClaimsPrincipal principal, AuthenticationProperties properties) {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var sessionId = principal.FindFirstValue(IndustrialAppStoreAuthenticationDefaults.AppSessionIdClaimType);
            if (tokenStore is DefaultTokenStore defaultStore) {
                await defaultStore.InitAsync(userId, sessionId, properties).ConfigureAwait(false);
            }
            else {
                await tokenStore.InitAsync(userId, sessionId).ConfigureAwait(false);
            }
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
