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
            AppBuilderUtils.ConfigureServices(Configuration, services);
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
            AppBuilderUtils.ConfigureApp(app, env);
        }
    }
}
