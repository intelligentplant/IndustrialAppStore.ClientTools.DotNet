using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ExampleMvcApplication {
    public class Startup {

        public IConfiguration Configuration { get; }


        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }


        /// <summary>
        /// Adds services to the container.
        /// </summary>
        /// <param name="services">
        ///   The service collection.
        /// </param>
        public void ConfigureServices(IServiceCollection services) {
            services.AddIndustrialAppStoreAuthentication(options => {
                // Bind the settings from the app configuration to the Industrial App Store 
                // authentication options.
                Configuration.GetSection("IAS").Bind(options);
                options.LoginPath = new PathString("/Account/Login");
            });

            services.AddControllersWithViews().AddNewtonsoftJson();
        }

        
        /// <summary>
        /// Configures the HTTP request pipeline.
        /// </summary>
        /// <param name="app">
        ///   The application builder.
        /// </param>
        /// <param name="env">
        ///   The web host environment.
        /// </param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
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
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
