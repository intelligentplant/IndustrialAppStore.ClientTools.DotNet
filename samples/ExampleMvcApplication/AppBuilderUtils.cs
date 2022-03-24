namespace Microsoft.Extensions.DependencyInjection {

    /// <summary>
    /// Utility methods for configuring the application.
    /// </summary>
    internal static class AppBuilderUtils {

        /// <summary>
        /// Adds services to the service collection.
        /// </summary>
        /// <param name="configuration">
        ///   The <see cref="IConfiguration"/>.
        /// </param>
        /// <param name="services">
        ///   The <see cref="IServiceCollection"/>.
        /// </param>
        internal static void ConfigureServices(IConfiguration configuration, IServiceCollection services) {
            services.AddCustomHeaders();
            services.AddContentSecurityPolicy();

            services.AddIndustrialAppStoreAuthentication(options => {
                // Bind the settings from the app configuration to the Industrial App Store 
                // authentication options.
                configuration.GetSection("IAS").Bind(options);

                // Redirect to our login page when an authentication challenge is issued.
                options.LoginPath = new PathString("/Account/Login");

                // The UseCookieSessionIdGenerator extension method configures the SessionIdGenerator
                // property to store a persistent device ID cookie in the calling user agent, so
                // that logins from the same browser will always use the same session ID. If
                // SessionIdGenerator is not configured, a new session ID will be generated for
                // every login.
                //options.UseCookieSessionIdGenerator();

                // The ConfigureHttpClient property can be used to customise the HttpClient that is
                // used for Data Core API calls e.g. 
                //options.ConfigureHttpClient = builder => builder.AddHttpMessageHandler<MyCustomHandler>();
            });

            if (configuration.GetValue<bool>("IAS:UseExternalAuthentication")) {
                // App is configured to use an authentication provider other than the Industrial
                // App Store, so we will use IIS to handle Windows authentication for us.
                services.AddAuthentication(Microsoft.AspNetCore.Server.IISIntegration.IISDefaults.AuthenticationScheme);
            }

            services.AddControllersWithViews().AddNewtonsoftJson();
        }


        /// <summary>
        /// Configures the middleware pipeline.
        /// </summary>
        /// <param name="app">
        ///   The <see cref="IApplicationBuilder"/>.
        /// </param>
        /// <param name="env">
        ///   The <see cref="IWebHostEnvironment"/>.
        /// </param>
        internal static void ConfigureApp(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCustomHeaders();
            app.UseContentSecurityPolicy();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapDefaultControllerRoute();
            });
        }

    }
}
