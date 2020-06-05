using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using DataCore.Adapter;
using DataCore.Adapter.Diagnostics;

using IntelligentPlant.BackgroundTasks;

using Microsoft.Extensions.Logging;

namespace ExampleAdapter {

    // This is your adapter class. For information about how to add features to your adapter (such 
    // as tag browsing, or real-time data queries), visit https://github.com/intelligentplant/app-store-connect-adapters.

    public class MyAdapter : AdapterBase<MyAdapterOptions> {

        public MyAdapter(
            string id, 
            MyAdapterOptions options,
            IBackgroundTaskService taskScheduler,
            ILogger<MyAdapter> logger
        ) : base(id, options, taskScheduler, logger) {  }


        // This constructor allows your adapter to respond to configuration changes at runtime. 
        // See the OnOptionsChange method below. You can remove this constructor if you do not 
        // require this capability.
        public MyAdapter(
            string id,
            IAdapterOptionsMonitor<MyAdapterOptions> options,
            IBackgroundTaskService taskScheduler,
            ILogger<MyAdapter> logger
        ) : base(id, options, taskScheduler, logger) { }


        // If the adapter is created using the IAdapterOptionsMonitor<T> overload above, the 
        // OnOptionsChange method will be called whenever the adapter's configuration options are 
        // changed once the adapter has started. You can remove this override if it is not required.
        protected override void OnOptionsChange(MyAdapterOptions options) {
            base.OnOptionsChange(options);
        }


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


        // Implement the CheckHealthAsync method to add custom health checks to your adapter. 
        // Health checks allow you to report on the status of e.g. connections to external 
        // systems.
        protected override async Task<IEnumerable<HealthCheckResult>> CheckHealthAsync(
            IAdapterCallContext context, 
            CancellationToken cancellationToken
        ) {
            if (!IsRunning) {
                return await base.CheckHealthAsync(context, cancellationToken).ConfigureAwait(false);
            }

            var result = new List<HealthCheckResult>();

            // Add custom health check results to the list.

            // CheckFeatureHealthAsync can be used to get health check results for adapter 
            // features implemented in external classes.
            result.AddRange(await CheckFeatureHealthAsync(context, cancellationToken).ConfigureAwait(false));

            return result;
        }


        // Override the Dispose(bool) method if you need to dispose of managed or unmanaged 
        // resources. If your resources implement IAsyncDisposable, you can also override 
        // DisposeAsync(bool) to provide both synchronous and asynchronous dispose methods.

    }
}
