using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace ExampleHostedAdapter {
    public class Startup {

        // Our adapter instance will use this ID at runtime.
        public const string AdapterId = "fdb421d7-03b2-49e8-880a-224e8e5f04ef";


        public IConfiguration Configuration { get; }


        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services) {
            // Add adapter services

            var version = GetType().Assembly.GetName().Version.ToString(3);

            services
                .AddDataCoreAdapterAspNetCoreServices()
                .AddHostInfo("ExampleHostedAdapter Host", "A host application for an App Store Connect adapter")
                .AddServices(svc => {
                    // Bind adapter options against the application configuration.
                    svc.Configure<ExampleHostedAdapterOptions>(AdapterId, Configuration.GetSection($"AppStoreConnect:Settings:{AdapterId}"));
                })
                // Register our adapter with the DI container. The AdapterId parameter will be
                // used as the adapter ID.
                .AddAdapter(sp => ActivatorUtilities.CreateInstance<ExampleHostedAdapter>(sp, AdapterId));
            //.AddAdapterFeatureAuthorization<MyAdapterFeatureAuthHandler>();

            // To add authentication and authorization for adapter API operations, extend 
            // the FeatureAuthorizationHandler class and call AddAdapterFeatureAuthorization
            // above to register your handler.

            services.AddGrpc();

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Latest)
                .AddJsonOptions(options => {
                    options.JsonSerializerOptions.WriteIndented = true;
                })
                .AddDataCoreAdapterMvc();

            // Add OpenTelemetry tracing
            services.AddOpenTelemetryTracing(builder => {
                builder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(
                        "IAS Adapter Host",
                        serviceVersion: version,
                        serviceInstanceId: AdapterId
                    ))
                    // Use AddConsoleExporter() to export to stdout. 
                    //.AddConsoleExporter()    
                    // Use AddJaegerExporter() to export to Jaeger (https://www.jaegertracing.io/).
                    //.AddJaegerExporter() 
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSqlClientInstrumentation()
                    .AddDataCoreAdapterInstrumentation();
            });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRequestLocalization();
            app.UseRouting();

            app.UseEndpoints(endpoints => {
                // Map MVC API and gRPC endpoints.
                endpoints.MapControllers();
                endpoints.MapDataCoreGrpcServices();

                // Redirect requests to / to the API endpoint for retrieving details about the
                // adapter.
                endpoints.MapGet("/", context => {
                    context.Response.Redirect($"/api/app-store-connect/v2.0/adapters/{AdapterId}");
                    return Task.CompletedTask;
                });
            });
        }
    }
}
