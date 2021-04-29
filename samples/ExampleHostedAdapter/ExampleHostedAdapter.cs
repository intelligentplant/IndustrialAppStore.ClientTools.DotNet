using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using DataCore.Adapter;
using DataCore.Adapter.Diagnostics;

using IntelligentPlant.BackgroundTasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

// The [VendorInfo] attribute is used to add vendor information to all adapters in this assembly.
[assembly: VendorInfo("My Company", "https://my-company.com")]

namespace ExampleHostedAdapter {

    // This is your adapter class. For information about how to add features to your adapter (such 
    // as tag browsing, or real-time data queries), visit https://github.com/intelligentplant/AppStoreConnect.Adapters.

    // The [AdapterMetadata] attribute is used to provide information about your adapter type at
    // runtime.
    [AdapterMetadata(
        // This is a URI to identify the adapter type; it is not required that the URI can be
        // dereferenced!
        "https://my-company.com/app-store-connect/adapters/my-adapter/",
        // The display name for the adapter type.
        Name = "My Adapter",
        // The adapter type description.
        Description = "A brief description of the adapter type"
    )]
    public partial class ExampleHostedAdapter : AdapterBase<ExampleHostedAdapterOptions> {

        public ExampleHostedAdapter(
            string id, 
            IOptionsMonitor<ExampleHostedAdapterOptions> options,
            IBackgroundTaskService taskScheduler,
            ILogger<ExampleHostedAdapter> logger
        ) : base(id, options, taskScheduler, logger) {  }


        // The StartAsync method is called when the adapter is being started up. Use this method to 
        // initialise any required connections to external systems (e.g. connecting to a database, 
        // MQTT broker, industrial plant historian, etc).
        protected override Task StartAsync(CancellationToken cancellationToken) {
            return Task.CompletedTask;
        }


        // The StopAsync method is called when the adapter is being shut down. Use this method to 
        // shut down connections to external systems. Note that an adapter can be stopped and 
        // started again without being disposed. You should override the Dispose and DisposeAsync 
        // methods to dispose of resources when the adapter is being disposed.
        protected override Task StopAsync(CancellationToken cancellationToken) {
            return Task.CompletedTask;
        }


        // The OnOptionsChange method is called every time the adapter receives an update to its
        // options at runtime. You can use this method to trigger any runtime changes required.
        // You can test this functionality by running the application and then changing the
        // adapter name or description in appsettings.json at runtime.
        protected override void OnOptionsChange(ExampleHostedAdapterOptions options) {
            base.OnOptionsChange(options);
        }


        // Override the CheckHealthAsync method to add custom health checks to your adapter. 
        // Health checks allow you to report on the status of e.g. connections to external 
        // systems. If you detect that the underlying health status of the adapter has changed 
        // (e.g. you unexpectedly disconnect from an external system) you can notify the base 
        // class that the overall health status must be recalculated by calling the 
        // OnHealthStatusChanged method.
        protected override async Task<IEnumerable<HealthCheckResult>> CheckHealthAsync(
            IAdapterCallContext context, 
            CancellationToken cancellationToken
        ) {
            var result = new List<HealthCheckResult>();
            result.AddRange(await base.CheckHealthAsync(context, cancellationToken).ConfigureAwait(false));

            // Use the IsRunning flag to detect if the adapter has been initialised.

            if (!IsRunning) {
                return result;
            }

            // Add custom health check results to the list.

            return result;
        }


        // Override the Dispose(bool) method if you need to dispose of managed or unmanaged 
        // resources. 
        //
        // If any of your resources implement IAsyncDisposable, you can also override 
        // DisposeAsyncCore() to implement asynchronous disposal. You should ensure that you also 
        // dispose of these resources synchronously in your Dispose(bool) implementation if the 
        // 'disposing' parameter is true.
        //
        // Note that, when calling DisposeAsync(), the behaviour implemented by the adapter is to 
        // call DisposeAsyncCore(), and then call Dispose(false). See here for more details: 
        // https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync

    }
}
