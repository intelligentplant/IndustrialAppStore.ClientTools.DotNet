using System.ComponentModel;

using IntelligentPlant.DataCore.Client;
using IntelligentPlant.IndustrialAppStore.Client;

using Microsoft.SemanticKernel;

namespace IntelligentPlant.IndustrialAppStore.SemanticKernel.Plugins {

    [Description("Defines functions for interacting with the Industrial App Store.")]
    internal class IndustrialAppStoreDataApiPlugin {

        public const string PluginName = "industrial_app_store";

        private readonly IndustrialAppStoreHttpClient _iasClient;

        private readonly TimeProvider _timeProvider;


        public IndustrialAppStoreDataApiPlugin(IndustrialAppStoreHttpClient iasClient, TimeProvider timeProvider) {
            _iasClient = iasClient;
            _timeProvider = timeProvider;
        }


        [KernelFunction("get_current_time")]
        [Description("Gets the current UTC date and time. Use this function when you need to know what time it is before calling a function that requires a time range.")]
        public DateTime GetCurrentTime(PluginRequest request) => _timeProvider.GetUtcNow().UtcDateTime;


        [KernelFunction("get_data_sources")]
        [Description("Gets the available Industrial App Store data sources.")]
        public async Task<IEnumerable<IasDataSourceDescriptor>> GetDataSourcesAsync(PluginRequest request, CancellationToken cancellationToken) {
            var result = await _iasClient.DataSources.GetDataSourcesAsync(cancellationToken);
            return result.Select(x => new IasDataSourceDescriptor() {
                Id = x.Name.QualifiedName!,
                DisplayName = x.Name.DisplayName,
                Description = x.Description
            }).ToArray();
        }


        [KernelFunction("find_tags")]
        [Description("Searches an Industrial App Store data source for tags (i.e. instruments or sensors) with names matching the provided filter on the data source with the specified identifier. The * character can be used in the name filter as a wildcard. The page parameter specifies the result page to return.")]
        public async Task<IEnumerable<IasTagDescriptor>> FindTagsAsync(FindTagsRequest request, CancellationToken cancellationToken) {
            var result = await _iasClient.DataSources.FindTagsAsync(new DataCore.Client.Queries.FindTagsRequest() {
                DataSourceName = request.DataSourceId,
                Filter = new DataCore.Client.Model.TagSearchFilter(request.Name, request.Description, request.Units) { 
                    Label = request.Labels,
                    PageSize = request.PageSize,
                    Page = request.Page
                }
            }, cancellationToken);

            return result.Select(x => new IasTagDescriptor() {
                Name = x.Name,
                Description = x.Description,
                Units = x.UnitOfMeasure,
                Labels = x.Labels?.ToArray(),
                States = x.DigitalStates?.Count > 0
                    ? x.DigitalStates.Select(s => new IasTagDiscreteState() { Name = s.Name, Value = s.Value }).ToArray()
                    : null
            }).ToArray();
        }


        [KernelFunction("read_current_values")]
        [Description("Reads the current values of the specified tags.")]
        public async Task<IEnumerable<IasDataValueCollection>> ReadCurrentValuesAsync(ReadCurrentValuesRequest request, CancellationToken cancellationToken) {
            var result = await _iasClient.DataSources.ReadSnapshotTagValuesAsync(request.DataSourceId, request.TagNames, cancellationToken: cancellationToken);

            return result!.Select(x => new IasDataValueCollection() {
                TagName = x.Key,
                Values = new[] {
                    IasDataValue.FromTagValue(x.Value)
                }
            }).ToArray();
        }


        [KernelFunction("read_raw_historical_values")]
        [Description("Reads raw historical values for the specified tags. Note that data for individual tags is unlikely to align exactly by timestamp due to the data filtering techniques applied when recording data.")]
        public async Task<IEnumerable<IasDataValueCollection>> ReadRawHistoricalValuesAsync(ReadRawHistoricalValuesRequest request, CancellationToken cancellationToken) {
            var result = await _iasClient.DataSources.ReadRawTagValuesAsync(request.DataSourceId, request.TagNames, request.StartTime, request.EndTime, request.MaxSamples, cancellationToken: cancellationToken);

            return result!.Select(x => new IasDataValueCollection() {
                TagName = x.Key,
                Values = x.Value.Values.Select(x => IasDataValue.FromTagValue(x)).ToArray()
            }).ToArray();
        }


        [KernelFunction("read_best_fit_historical_values")]
        [Description("Reads a best-fit set of raw historical values for the specified tags for display purposes. This may also be referred to as a plot query. Note that data for individual tags is unlikely to align exactly by timestamp due to the data filtering techniques applied when recording data.")]
        public async Task<IEnumerable<IasDataValueCollection>> ReadBestFitHistoricalValuesAsync(ReadBestFitHistoricalValuesRequest request, CancellationToken cancellationToken) {
            var result = await _iasClient.DataSources.ReadPlotTagValuesAsync(request.DataSourceId, request.TagNames, request.StartTime, request.EndTime, request.Width, cancellationToken: cancellationToken);

            return result!.Select(x => new IasDataValueCollection() {
                TagName = x.Key,
                Values = x.Value.Values.Select(x => IasDataValue.FromTagValue(x)).ToArray()
            }).ToArray();
        }


        [KernelFunction("read_processed_historical_values")]
        [Description("Reads aggregated historical values for the specified tags.")]
        public async Task<IEnumerable<IasDataValueCollection>> ReadProcessedHistoricalValuesAsync(ReadProcessedHistoricalValuesRequest request, CancellationToken cancellationToken) {
            var result = await _iasClient.DataSources.ReadProcessedTagValuesAsync(request.DataSourceId, request.TagNames, request.StartTime, request.EndTime, request.DataFunction, request.SampleInterval, cancellationToken: cancellationToken);

            return result!.Select(x => new IasDataValueCollection() {
                TagName = x.Key,
                Values = x.Value.Values.Select(x => IasDataValue.FromTagValue(x)).ToArray()
            }).ToArray();
        }

    }

}
