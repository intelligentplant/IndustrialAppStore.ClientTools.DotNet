using ExampleMvcApplication.Services;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

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
            // Retrieves and logs group memberships in the background when a user logs into the
            // app.
            services.AddHostedService<BackgroundGroupFetcher>();

            // Configure in-memory SQLite connection (for example purposes only; use an on-disk
            // database connection in the real world!)
            services.AddDbContext<UserTokensDbContext>(options => {
                var conn = new SqliteConnection("Data Source=EFTokenStore;Mode=Memory;Cache=Shared");
                conn.Open();
                options.UseSqlite(conn);
            });

            services.AddCustomHeaders();

            services.AddIndustrialAppStoreAuthentication(options => {
                // Bind the settings from the app configuration to the Industrial App Store 
                // authentication options.
                configuration.GetSection("IAS").Bind(options);

                // Specify the path to be our login page.
                options.LoginPath = new PathString("/Account/Login");

                // The IndustrialAppStoreAuthenticationOptions.ConfigureHttpClient property can be
                // used to customise the HttpClient that is used for Data Core API calls e.g. 
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
            app.UseCustomHeaders();

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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapDefaultControllerRoute();
            });
        }

    }
}
