using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IntelligentPlant.DataCore.Client.Clients;
using IntelligentPlant.DataCore.Client.Queries;
using IntelligentPlant.DataCore.Client.Model;
using IntelligentPlant.DataCore.Client.Model.Scripting;

namespace IntelligentPlant.DataCore.Client {

    /// <summary>
    /// Extensions for <see cref="DataCoreHttpClient"/>.
    /// </summary>
    public static class DataCoreHttpClientExtensions {

        #region [ Helper Methods ]

        /// <summary>
        /// Converts an absolute or relative time stamp into a UTC <see cref="DateTime"/>.
        /// </summary>
        /// <param name="absoluteOrRelativeTime">
        ///   The absolute or relative time stamp literal.
        /// </param>
        /// <param name="cultureInfo">
        ///   The <see cref="CultureInfo"/> to use for conversions.
        /// </param>
        /// <param name="timeZone">
        ///   The time zone that <paramref name="absoluteOrRelativeTime"/> is assumed to be in. 
        ///   Specify <see langword="null"/> to assume local time.
        /// </param>
        /// <param name="parameterName">
        ///   The name of the parameter in the calling method that is being parsed. This will be 
        ///   included in the <see cref="ArgumentException"/> thrown if <paramref name="absoluteOrRelativeTime"/> 
        ///   cannot be converted to a <see cref="DateTime"/>.
        /// </param>
        /// <param name="utcTime">
        ///   The parsed UTC <see cref="DateTime"/>.
        /// </param>
        private static void ParseTimeStamp(string absoluteOrRelativeTime, CultureInfo cultureInfo, TimeZoneInfo timeZone, string parameterName, out DateTime utcTime) {
            if (!IntelligentPlant.Relativity.RelativityParser.TryGetParser(cultureInfo, out var parser)) {
                parser = IntelligentPlant.Relativity.RelativityParser.Default;
            }
            
            if (!parser.TryConvertToUtcDateTime(absoluteOrRelativeTime, out utcTime, timeZone ?? TimeZoneInfo.Local)) {
                throw new ArgumentException(Resources.Error_InvalidTimeStamp, parameterName);
            }
        }


        /// <summary>
        /// Parses a long-hand or short-hand time span literal into a 
        /// <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="sampleInterval">
        ///   The long-hand or short-hand time span literal.
        /// </param>
        /// <param name="cultureInfo">
        ///   The <see cref="CultureInfo"/> to use for conversions.
        /// </param>
        /// <param name="ts">
        ///   The parsed <see cref="TimeSpan"/>.
        /// </param>
        private static void ParseSampleInterval(string sampleInterval, CultureInfo cultureInfo, out TimeSpan ts) {
            if (!IntelligentPlant.Relativity.RelativityParser.TryGetParser(cultureInfo, out var parser)) {
                parser = IntelligentPlant.Relativity.RelativityParser.Default;
            }

            if (!parser.TryConvertToTimeSpan(sampleInterval, out ts)) {
                throw new ArgumentException(Resources.Error_InvalidSampleInterval, nameof(sampleInterval));
            }
        }

        #endregion

        #region [ Tag Searches ]

        /// <summary>
        /// Finds tags on the specified data source.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The context type that is passed to API calls to allow authentication headers to be added 
        ///   to outgoing requests.
        /// </typeparam>
        /// <typeparam name="TOptions">
        ///   The options type for the client.
        /// </typeparam>
        /// <param name="client">
        ///   The Data Core client.
        /// </param>
        /// <param name="dataSourceName">
        ///   The data source name.
        /// </param>
        /// <param name="nameFilter">
        ///   The tag name filter.
        /// </param>
        /// <param name="descriptionFilter">
        ///   The tag description filter.
        /// </param>
        /// <param name="unitsFilter">
        ///   The tag units filter.
        /// </param>
        /// <param name="page">
        ///   The query result page to retrieve.
        /// </param>
        /// <param name="pageSize">
        ///   The page size for the query.
        /// </param>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, 
        ///   this will be passed to the handler's callback when requesting the <c>Authorize</c> 
        ///   header value for the outgoing request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The search results.
        /// </returns>
        public static Task<IEnumerable<TagSearchResult>> FindTagsAsync<TContext, TOptions>(
            this DataSourcesClient<TContext, TOptions> client, 
            string dataSourceName, 
            string nameFilter = "*", 
            string descriptionFilter = null, 
            string unitsFilter = null,
            int page = 1,
            int pageSize = 20,
            TContext context = default, 
            CancellationToken cancellationToken = default
        ) where TOptions : DataCoreHttpClientOptions {

            if (client == null) {
                throw new ArgumentNullException(nameof(client));
            }

            return client.FindTagsAsync(
                new FindTagsRequest() {
                    DataSourceName = dataSourceName,
                    Filter = new TagSearchFilter(nameFilter, descriptionFilter, unitsFilter) {
                        PageSize = pageSize,
                        Page = page
                    }
                }, 
                context, 
                cancellationToken
            );
        }


        /// <summary>
        /// Finds script tags on the specified data source.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The context type that is passed to API calls to allow authentication headers to be added 
        ///   to outgoing requests.
        /// </typeparam>
        /// <typeparam name="TOptions">
        ///   The options type for the client.
        /// </typeparam>
        /// <param name="client">
        ///   The Data Core client.
        /// </param>
        /// <param name="dataSourceName">
        ///   The data source name.
        /// </param>
        /// <param name="nameFilter">
        ///   The tag name filter.
        /// </param>
        /// <param name="descriptionFilter">
        ///   The tag description filter.
        /// </param>
        /// <param name="unitsFilter">
        ///   The tag units filter.
        /// </param>
        /// <param name="page">
        ///   The query result page to retrieve.
        /// </param>
        /// <param name="pageSize">
        ///   The page size for the query.
        /// </param>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, 
        ///   this will be passed to the handler's callback when requesting the <c>Authorize</c> 
        ///   header value for the outgoing request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The search results.
        /// </returns>
        public static Task<IEnumerable<ScriptTagDefinition>> FindScriptTagsAsync<TContext, TOptions>(
            this DataSourcesClient<TContext, TOptions> client,
            string dataSourceName,
            string nameFilter = "*",
            string descriptionFilter = null,
            string unitsFilter = null,
            int page = 1,
            int pageSize = 20,
            TContext context = default,
            CancellationToken cancellationToken = default
        ) where TOptions : DataCoreHttpClientOptions {

            if (client == null) {
                throw new ArgumentNullException(nameof(client));
            }

            return client.FindScriptTagsAsync(
                new FindTagsRequest() {
                    DataSourceName = dataSourceName,
                    Filter = new TagSearchFilter(nameFilter, descriptionFilter, unitsFilter) {
                        PageSize = pageSize,
                        Page = page
                    }
                },
                context,
                cancellationToken
            );
        }


        /// <summary>
        /// Gets script tags on the specified data source.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The context type that is passed to API calls to allow authentication headers to be added 
        ///   to outgoing requests.
        /// </typeparam>
        /// <typeparam name="TOptions">
        ///   The options type for the client.
        /// </typeparam>
        /// <param name="client">
        ///   The Data Core client.
        /// </param>
        /// <param name="dataSourceName">
        ///   The data source name.
        /// </param>
        /// <param name="namesOrIds">
        ///   The names or IDs of the script tags to retrieve.
        /// </param>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, 
        ///   this will be passed to the handler's callback when requesting the <c>Authorize</c> 
        ///   header value for the outgoing request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The search results.
        /// </returns>
        public static Task<IEnumerable<ScriptTagDefinition>> GetScriptTagsAsync<TContext, TOptions>(
            this DataSourcesClient<TContext, TOptions> client,
            string dataSourceName,
            IEnumerable<string> namesOrIds,
            TContext context = default,
            CancellationToken cancellationToken = default
        ) where TOptions : DataCoreHttpClientOptions {

            if (client == null) {
                throw new ArgumentNullException(nameof(client));
            }

            return client.GetScriptTagsAsync(
                new GetTagsRequest() { 
                    DataSourceName = dataSourceName,
                    TagNamesOrIds = namesOrIds?.ToArray()
                },
                context,
                cancellationToken
            );
        }


        /// <summary>
        /// Gets a single script tag on the specified data source.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The context type that is passed to API calls to allow authentication headers to be added 
        ///   to outgoing requests.
        /// </typeparam>
        /// <typeparam name="TOptions">
        ///   The options type for the client.
        /// </typeparam>
        /// <param name="client">
        ///   The Data Core client.
        /// </param>
        /// <param name="dataSourceName">
        ///   The data source name.
        /// </param>
        /// <param name="nameOrId">
        ///   The name or ID of the script tag to retrieve.
        /// </param>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, 
        ///   this will be passed to the handler's callback when requesting the <c>Authorize</c> 
        ///   header value for the outgoing request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The search results.
        /// </returns>
        public static Task<ScriptTagDefinition> GetScriptTagAsync<TContext, TOptions>(
            this DataSourcesClient<TContext, TOptions> client,
            string dataSourceName,
            string nameOrId,
            TContext context = default,
            CancellationToken cancellationToken = default
        ) where TOptions : DataCoreHttpClientOptions {

            if (client == null) {
                throw new ArgumentNullException(nameof(client));
            }

            return client.GetScriptTagAsync(
                new GetTagRequest() {
                    DataSourceName = dataSourceName,
                    TagNameOrId = nameOrId
                },
                context,
                cancellationToken
            );
        }

        #endregion

        #region [ NOW Tag Value Queries ]

        /// <summary>
        /// Gets snapshot (current) tag values.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The context type that is passed to API calls to allow authentication headers to be added 
        ///   to outgoing requests.
        /// </typeparam>
        /// <typeparam name="TOptions">
        ///   The options type for the client.
        /// </typeparam>
        /// <param name="client">
        ///   The Data Core client.
        /// </param>
        /// <param name="tagMap">
        ///   A dictionary of tags to query, mapping from data source name to tag names.
        /// </param>
        /// <param name="properties">
        ///   Bespoke query properties.
        /// </param>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, 
        ///   this will be passed to the handler's callback when requesting the <c>Authorize</c> 
        ///   header value for the outgoing request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The snapshot tag values, indexed by data source name and then tag name.
        /// </returns>
        public static Task<IDictionary<string, SnapshotTagValueDictionary>> ReadSnapshotTagValuesAsync<TContext, TOptions>(
            this DataSourcesClient<TContext, TOptions> client,
            IDictionary<string, string[]> tagMap,
            IDictionary<string, string> properties = null,
            TContext context = default,
            CancellationToken cancellationToken = default
        ) where TOptions : DataCoreHttpClientOptions {
            if (client == null) {
                throw new ArgumentNullException(nameof(client));
            }

            return client.ReadSnapshotTagValuesAsync(
                new ReadSnapshotTagValuesRequest() {
                    Tags = tagMap,
                    QueryProperties = properties
                },
                context,
                cancellationToken
            );
        }


        /// <summary>
        /// Gets snapshot (current) tag values.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The context type that is passed to API calls to allow authentication headers to be added 
        ///   to outgoing requests.
        /// </typeparam>
        /// <typeparam name="TOptions">
        ///   The options type for the client.
        /// </typeparam>
        /// <param name="client">
        ///   The Data Core client.
        /// </param>
        /// <param name="dataSourceName">
        ///   The data source name to query.
        /// </param>
        /// <param name="tagNames">
        ///   The tag names to query.
        /// </param>
        /// <param name="properties">
        ///   Bespoke query properties.
        /// </param>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, 
        ///   this will be passed to the handler's callback when requesting the <c>Authorize</c> 
        ///   header value for the outgoing request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The snapshot tag values, indexed by tag name.
        /// </returns>
        public static async Task<SnapshotTagValueDictionary> ReadSnapshotTagValuesAsync<TContext, TOptions>(
            this DataSourcesClient<TContext, TOptions> client,
            string dataSourceName,
            IEnumerable<string> tagNames,
            IDictionary<string, string> properties = null,
            TContext context = default,
            CancellationToken cancellationToken = default
        ) where TOptions : DataCoreHttpClientOptions {
            if (client == null) {
                throw new ArgumentNullException(nameof(client));
            }

            if (string.IsNullOrWhiteSpace(dataSourceName)) {
                throw new ArgumentException(Resources.Error_DataSourceNameIsRequired, nameof(dataSourceName));
            }

            var result = await client.ReadSnapshotTagValuesAsync(
                new ReadSnapshotTagValuesRequest() {
                    Tags = new Dictionary<string, string[]>() {
                        { dataSourceName, tagNames?.Distinct()?.ToArray() }
                    },
                    QueryProperties = properties
                },
                context,
                cancellationToken
            ).ConfigureAwait(false);

            return result?.FirstOrDefault().Value;
        }

        #endregion

        #region [ RAW Tag Value Queries ]

        /// <summary>
        /// Gets raw historical tag values.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The context type that is passed to API calls to allow authentication headers to be added 
        ///   to outgoing requests.
        /// </typeparam>
        /// <typeparam name="TOptions">
        ///   The options type for the client.
        /// </typeparam>
        /// <param name="client">
        ///   The Data Core client.
        /// </param>
        /// <param name="tagMap">
        ///   A dictionary of tags to query, mapping from data source name to tag names.
        /// </param>
        /// <param name="utcStartTime">
        ///   The UTC start time for the query.
        /// </param>
        /// <param name="utcEndTime">
        ///   The UTC end time for the query.
        /// </param>
        /// <param name="pointCount">
        ///   The maximum number of values to return per tag. The data source may apply a more 
        ///   restrictive limit.
        /// </param>
        /// <param name="properties">
        ///   Bespoke query properties.
        /// </param>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, 
        ///   this will be passed to the handler's callback when requesting the <c>Authorize</c> 
        ///   header value for the outgoing request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The historical tag values, indexed by data source name and then tag name.
        /// </returns>
        public static Task<IDictionary<string, HistoricalTagValuesDictionary>> ReadRawTagValuesAsync<TContext, TOptions>(
            this DataSourcesClient<TContext, TOptions> client,
            IDictionary<string, string[]> tagMap,
            DateTime utcStartTime,
            DateTime utcEndTime,
            int pointCount,
            IDictionary<string, string> properties = null,
            TContext context = default,
            CancellationToken cancellationToken = default
        ) where TOptions : DataCoreHttpClientOptions {

            if (client == null) {
                throw new ArgumentNullException(nameof(client));
            }

            return client.ReadRawTagValuesAsync(
                new ReadRawTagValuesRequest() {
                    Tags = tagMap,
                    StartTime = utcStartTime,
                    EndTime = utcEndTime,
                    PointCount = pointCount,
                    QueryProperties = properties
                },
                context,
                cancellationToken
            );
        }


        /// <summary>
        /// Gets raw historical tag values.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The context type that is passed to API calls to allow authentication headers to be added 
        ///   to outgoing requests.
        /// </typeparam>
        /// <typeparam name="TOptions">
        ///   The options type for the client.
        /// </typeparam>
        /// <param name="client">
        ///   The Data Core client.
        /// </param>
        /// <param name="tagMap">
        ///   A dictionary of tags to query, mapping from data source name to tag names.
        /// </param>
        /// <param name="absoluteOrRelativeStartTime">
        ///   The absolute or relative start time for the query.
        /// </param>
        /// <param name="absoluteOrRelativeEndTime">
        ///   The absolute or relative end time for the query.
        /// </param>
        /// <param name="pointCount">
        ///   The maximum number of values to return per tag. The data source may apply a more 
        ///   restrictive limit.
        /// </param>
        /// <param name="properties">
        ///   Bespoke query properties.
        /// </param>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, 
        ///   this will be passed to the handler's callback when requesting the <c>Authorize</c> 
        ///   header value for the outgoing request.
        /// </param>
        /// <param name="cultureInfo">
        ///   The <see cref="CultureInfo"/> to use in relative timestamp conversions.
        /// </param>
        /// <param name="timeZone">
        ///   The assumed time zone in relative timestamp conversions. Specify <see langword="null"/>
        ///   to assume UTC.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The historical tag values, indexed by data source name and then tag name.
        /// </returns>
        public static Task<IDictionary<string, HistoricalTagValuesDictionary>> ReadRawTagValuesAsync<TContext, TOptions>(
            this DataSourcesClient<TContext, TOptions> client,
            IDictionary<string, string[]> tagMap,
            string absoluteOrRelativeStartTime,
            string absoluteOrRelativeEndTime,
            int pointCount,
            IDictionary<string, string> properties = null,
            TContext context = default,
            CultureInfo cultureInfo = null,
            TimeZoneInfo timeZone = null,
            CancellationToken cancellationToken = default
        ) where TOptions : DataCoreHttpClientOptions {

            ParseTimeStamp(absoluteOrRelativeStartTime, cultureInfo, timeZone, nameof(absoluteOrRelativeStartTime), out var utcStartTime);
            ParseTimeStamp(absoluteOrRelativeEndTime, cultureInfo, timeZone, nameof(absoluteOrRelativeEndTime), out var utcEndTime);

            return ReadRawTagValuesAsync(
                client,
                tagMap,
                utcStartTime,
                utcEndTime,
                pointCount,
                properties,
                context,
                cancellationToken
            );
        }


        /// <summary>
        /// Gets raw historical tag values.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The context type that is passed to API calls to allow authentication headers to be added 
        ///   to outgoing requests.
        /// </typeparam>
        /// <typeparam name="TOptions">
        ///   The options type for the client.
        /// </typeparam>
        /// <param name="client">
        ///   The Data Core client.
        /// </param>
        /// <param name="dataSourceName">
        ///   The data source to query.
        /// </param>
        /// <param name="tagNames">
        ///   The tags to query.
        /// </param>
        /// <param name="utcStartTime">
        ///   The UTC start time for the query.
        /// </param>
        /// <param name="utcEndTime">
        ///   The UTC end time for the query.
        /// </param>
        /// <param name="pointCount">
        ///   The maximum number of values to return per tag. The data source may apply a more 
        ///   restrictive limit.
        /// </param>
        /// <param name="properties">
        ///   Bespoke query properties.
        /// </param>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, 
        ///   this will be passed to the handler's callback when requesting the <c>Authorize</c> 
        ///   header value for the outgoing request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The historical tag values, indexed by data source name and then tag name.
        /// </returns>
        public static async Task<HistoricalTagValuesDictionary> ReadRawTagValuesAsync<TContext, TOptions>(
            this DataSourcesClient<TContext, TOptions> client,
            string dataSourceName,
            IEnumerable<string> tagNames,
            DateTime utcStartTime,
            DateTime utcEndTime,
            int pointCount,
            IDictionary<string, string> properties = null,
            TContext context = default,
            CancellationToken cancellationToken = default
        ) where TOptions : DataCoreHttpClientOptions {

            if (string.IsNullOrWhiteSpace(dataSourceName)) {
                throw new ArgumentException(Resources.Error_DataSourceNameIsRequired, nameof(dataSourceName));
            }

            var result = await ReadRawTagValuesAsync(
                client,
                new Dictionary<string, string[]>() {
                        { dataSourceName, tagNames?.Distinct()?.ToArray() }
                    },
                utcStartTime,
                utcEndTime,
                pointCount,
                properties,
                context,
                cancellationToken
            ).ConfigureAwait(false);

            return result?.FirstOrDefault().Value;
        }


        /// <summary>
        /// Gets raw historical tag values.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The context type that is passed to API calls to allow authentication headers to be added 
        ///   to outgoing requests.
        /// </typeparam>
        /// <typeparam name="TOptions">
        ///   The options type for the client.
        /// </typeparam>
        /// <param name="client">
        ///   The Data Core client.
        /// </param>
        /// <param name="dataSourceName">
        ///   The data source to query.
        /// </param>
        /// <param name="tagNames">
        ///   The tags to query.
        /// </param>
        /// <param name="absoluteOrRelativeStartTime">
        ///   The absolute or relative start time for the query.
        /// </param>
        /// <param name="absoluteOrRelativeEndTime">
        ///   The absolute or relative end time for the query.
        /// </param>
        /// <param name="pointCount">
        ///   The maximum number of values to return per tag. The data source may apply a more 
        ///   restrictive limit.
        /// </param>
        /// <param name="properties">
        ///   Bespoke query properties.
        /// </param>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, 
        ///   this will be passed to the handler's callback when requesting the <c>Authorize</c> 
        ///   header value for the outgoing request.
        /// </param>
        /// <param name="cultureInfo">
        ///   The <see cref="CultureInfo"/> to use in relative timestamp conversions.
        /// </param>
        /// <param name="timeZone">
        ///   The assumed time zone in relative timestamp conversions. Specify <see langword="null"/>
        ///   to assume UTC.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The historical tag values, indexed by data source name and then tag name.
        /// </returns>
        public static Task<HistoricalTagValuesDictionary> ReadRawTagValuesAsync<TContext, TOptions>(
            this DataSourcesClient<TContext, TOptions> client,
            string dataSourceName,
            IEnumerable<string> tagNames,
            string absoluteOrRelativeStartTime,
            string absoluteOrRelativeEndTime,
            int pointCount,
            IDictionary<string, string> properties = null,
            TContext context = default,
            CultureInfo cultureInfo = null,
            TimeZoneInfo timeZone = null,
            CancellationToken cancellationToken = default
        ) where TOptions : DataCoreHttpClientOptions {

            ParseTimeStamp(absoluteOrRelativeStartTime, cultureInfo, timeZone, nameof(absoluteOrRelativeStartTime), out var utcStartTime);
            ParseTimeStamp(absoluteOrRelativeEndTime, cultureInfo, timeZone, nameof(absoluteOrRelativeEndTime), out var utcEndTime);

            return ReadRawTagValuesAsync(
                client, 
                dataSourceName, 
                tagNames, 
                utcStartTime, 
                utcEndTime, 
                pointCount, 
                properties, 
                context, 
                cancellationToken
            );
        }

        #endregion

        #region [ PLOT Tag Value Queries ]

        /// <summary>
        /// Gets visualization-friendly (plot) historical tag values, suitable for displaying on a 
        /// chart.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The context type that is passed to API calls to allow authentication headers to be added 
        ///   to outgoing requests.
        /// </typeparam>
        /// <typeparam name="TOptions">
        ///   The options type for the client.
        /// </typeparam>
        /// <param name="client">
        ///   The Data Core client.
        /// </param>
        /// <param name="tagMap">
        ///   A dictionary of tags to query, mapping from data source name to tag names.
        /// </param>
        /// <param name="utcStartTime">
        ///   The UTC start time for the query.
        /// </param>
        /// <param name="utcEndTime">
        ///   The UTC end time for the query.
        /// </param>
        /// <param name="intervals">
        ///   The number of intervals to use in the query. Plot queries work by dividing the time 
        ///   range into an equal number of intervals, and then selecting significant raw values 
        ///   from each interval. Therefore, the higher the interval count, the more values will 
        ///   be returned. The implementation of plot queries is left to the historian vendor, 
        ///   but a good rule of thumb is that up to 4 samples might be returned in each interval 
        ///   (typically the earliest, latest, minimum and maximum values).
        /// </param>
        /// <param name="properties">
        ///   Bespoke query properties.
        /// </param>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, 
        ///   this will be passed to the handler's callback when requesting the <c>Authorize</c> 
        ///   header value for the outgoing request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The historical tag values, indexed by data source name and then tag name.
        /// </returns>
        public static Task<IDictionary<string, HistoricalTagValuesDictionary>> ReadPlotTagValuesAsync<TContext, TOptions>(
            this DataSourcesClient<TContext, TOptions> client,
            IDictionary<string, string[]> tagMap,
            DateTime utcStartTime,
            DateTime utcEndTime,
            int intervals,
            IDictionary<string, string> properties = null,
            TContext context = default,
            CancellationToken cancellationToken = default
        ) where TOptions : DataCoreHttpClientOptions {

            if (client == null) {
                throw new ArgumentNullException(nameof(client));
            }

            return client.ReadPlotTagValuesAsync(
                new ReadPlotTagValuesRequest() {
                    Tags = tagMap,
                    StartTime = utcStartTime,
                    EndTime = utcEndTime,
                    Intervals = intervals,
                    QueryProperties = properties
                },
                context,
                cancellationToken
            );
        }


        /// <summary>
        /// Gets visualization-friendly (plot) historical tag values, suitable for displaying on a 
        /// chart.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The context type that is passed to API calls to allow authentication headers to be added 
        ///   to outgoing requests.
        /// </typeparam>
        /// <typeparam name="TOptions">
        ///   The options type for the client.
        /// </typeparam>
        /// <param name="client">
        ///   The Data Core client.
        /// </param>
        /// <param name="tagMap">
        ///   A dictionary of tags to query, mapping from data source name to tag names.
        /// </param>
        /// <param name="absoluteOrRelativeStartTime">
        ///   The absolute or relative start time for the query.
        /// </param>
        /// <param name="absoluteOrRelativeEndTime">
        ///   The absolute or relative end time for the query.
        /// </param>
        /// <param name="intervals">
        ///   The number of intervals to use in the query. Plot queries work by dividing the time 
        ///   range into an equal number of intervals, and then selecting significant raw values 
        ///   from each interval. Therefore, the higher the interval count, the more values will 
        ///   be returned. The implementation of plot queries is left to the historian vendor, 
        ///   but a good rule of thumb is that up to 4 samples might be returned in each interval 
        ///   (typically the earliest, latest, minimum and maximum values).
        /// </param>
        /// <param name="properties">
        ///   Bespoke query properties.
        /// </param>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, 
        ///   this will be passed to the handler's callback when requesting the <c>Authorize</c> 
        ///   header value for the outgoing request.
        /// </param>
        /// <param name="cultureInfo">
        ///   The <see cref="CultureInfo"/> to use in relative timestamp conversions.
        /// </param>
        /// <param name="timeZone">
        ///   The assumed time zone in relative timestamp conversions. Specify <see langword="null"/>
        ///   to assume UTC.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The historical tag values, indexed by data source name and then tag name.
        /// </returns>
        public static Task<IDictionary<string, HistoricalTagValuesDictionary>> ReadPlotTagValuesAsync<TContext, TOptions>(
            this DataSourcesClient<TContext, TOptions> client,
            IDictionary<string, string[]> tagMap,
            string absoluteOrRelativeStartTime,
            string absoluteOrRelativeEndTime,
            int intervals,
            IDictionary<string, string> properties = null,
            TContext context = default,
            CultureInfo cultureInfo = null,
            TimeZoneInfo timeZone = null,
            CancellationToken cancellationToken = default
        ) where TOptions : DataCoreHttpClientOptions {

            ParseTimeStamp(absoluteOrRelativeStartTime, cultureInfo, timeZone, nameof(absoluteOrRelativeStartTime), out var utcStartTime);
            ParseTimeStamp(absoluteOrRelativeEndTime, cultureInfo, timeZone, nameof(absoluteOrRelativeEndTime), out var utcEndTime);

            return ReadPlotTagValuesAsync(
                client,
                tagMap,
                utcStartTime,
                utcEndTime,
                intervals,
                properties,
                context,
                cancellationToken
            );
        }


        /// <summary>
        /// Gets visualization-friendly (plot) historical tag values, suitable for displaying on a 
        /// chart.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The context type that is passed to API calls to allow authentication headers to be added 
        ///   to outgoing requests.
        /// </typeparam>
        /// <typeparam name="TOptions">
        ///   The options type for the client.
        /// </typeparam>
        /// <param name="client">
        ///   The Data Core client.
        /// </param>
        /// <param name="dataSourceName">
        ///   The data source to query.
        /// </param>
        /// <param name="tagNames">
        ///   The tags to query.
        /// </param>
        /// <param name="utcStartTime">
        ///   The UTC start time for the query.
        /// </param>
        /// <param name="utcEndTime">
        ///   The UTC end time for the query.
        /// </param>
        /// <param name="intervals">
        ///   The number of intervals to use in the query. Plot queries work by dividing the time 
        ///   range into an equal number of intervals, and then selecting significant raw values 
        ///   from each interval. Therefore, the higher the interval count, the more values will 
        ///   be returned. The implementation of plot queries is left to the historian vendor, 
        ///   but a good rule of thumb is that up to 4 samples might be returned in each interval 
        ///   (typically the earliest, latest, minimum and maximum values).
        /// </param>
        /// <param name="properties">
        ///   Bespoke query properties.
        /// </param>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, 
        ///   this will be passed to the handler's callback when requesting the <c>Authorize</c> 
        ///   header value for the outgoing request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The historical tag values, indexed by data source name and then tag name.
        /// </returns>
        public static async Task<HistoricalTagValuesDictionary> ReadPlotTagValuesAsync<TContext, TOptions>(
            this DataSourcesClient<TContext, TOptions> client,
            string dataSourceName,
            IEnumerable<string> tagNames,
            DateTime utcStartTime,
            DateTime utcEndTime,
            int intervals,
            IDictionary<string, string> properties = null,
            TContext context = default,
            CancellationToken cancellationToken = default
        ) where TOptions : DataCoreHttpClientOptions {

            if (string.IsNullOrWhiteSpace(dataSourceName)) {
                throw new ArgumentException(Resources.Error_DataSourceNameIsRequired, nameof(dataSourceName));
            }

            var result = await ReadPlotTagValuesAsync(
                client,
                new Dictionary<string, string[]>() {
                        { dataSourceName, tagNames?.Distinct()?.ToArray() }
                    },
                utcStartTime,
                utcEndTime,
                intervals,
                properties,
                context,
                cancellationToken
            ).ConfigureAwait(false);

            return result?.FirstOrDefault().Value;
        }


        /// <summary>
        /// Gets visualization-friendly (plot) historical tag values, suitable for displaying on a 
        /// chart.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The context type that is passed to API calls to allow authentication headers to be added 
        ///   to outgoing requests.
        /// </typeparam>
        /// <typeparam name="TOptions">
        ///   The options type for the client.
        /// </typeparam>
        /// <param name="client">
        ///   The Data Core client.
        /// </param>
        /// <param name="dataSourceName">
        ///   The data source to query.
        /// </param>
        /// <param name="tagNames">
        ///   The tags to query.
        /// </param>
        /// <param name="absoluteOrRelativeStartTime">
        ///   The absolute or relative start time for the query.
        /// </param>
        /// <param name="absoluteOrRelativeEndTime">
        ///   The absolute or relative end time for the query.
        /// </param>
        /// <param name="intervals">
        ///   The number of intervals to use in the query. Plot queries work by dividing the time 
        ///   range into an equal number of intervals, and then selecting significant raw values 
        ///   from each interval. Therefore, the higher the interval count, the more values will 
        ///   be returned. The implementation of plot queries is left to the historian vendor, 
        ///   but a good rule of thumb is that up to 4 samples might be returned in each interval 
        ///   (typically the earliest, latest, minimum and maximum values).
        /// </param>
        /// <param name="properties">
        ///   Bespoke query properties.
        /// </param>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, 
        ///   this will be passed to the handler's callback when requesting the <c>Authorize</c> 
        ///   header value for the outgoing request.
        /// </param>
        /// <param name="cultureInfo">
        ///   The <see cref="CultureInfo"/> to use in relative timestamp conversions.
        /// </param>
        /// <param name="timeZone">
        ///   The assumed time zone in relative timestamp conversions. Specify <see langword="null"/>
        ///   to assume UTC.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The historical tag values, indexed by data source name and then tag name.
        /// </returns>
        public static Task<HistoricalTagValuesDictionary> ReadPlotTagValuesAsync<TContext, TOptions>(
            this DataSourcesClient<TContext, TOptions> client,
            string dataSourceName,
            IEnumerable<string> tagNames,
            string absoluteOrRelativeStartTime,
            string absoluteOrRelativeEndTime,
            int intervals,
            IDictionary<string, string> properties = null,
            TContext context = default,
            CultureInfo cultureInfo = null,
            TimeZoneInfo timeZone = null,
            CancellationToken cancellationToken = default
        ) where TOptions : DataCoreHttpClientOptions {

            ParseTimeStamp(absoluteOrRelativeStartTime, cultureInfo, timeZone, nameof(absoluteOrRelativeStartTime), out var utcStartTime);
            ParseTimeStamp(absoluteOrRelativeEndTime, cultureInfo, timeZone, nameof(absoluteOrRelativeEndTime), out var utcEndTime);

            return ReadPlotTagValuesAsync(
                client,
                dataSourceName,
                tagNames,
                utcStartTime,
                utcEndTime,
                intervals,
                properties,
                context,
                cancellationToken
            );
        }

        #endregion

        #region [ INTERP/AVG/MIN/MAX Tag Value Queries ]

        /// <summary>
        /// Gets processed (aggregated) historical tag values.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The context type that is passed to API calls to allow authentication headers to be added 
        ///   to outgoing requests.
        /// </typeparam>
        /// <typeparam name="TOptions">
        ///   The options type for the client.
        /// </typeparam>
        /// <param name="client">
        ///   The Data Core client.
        /// </param>
        /// <param name="tagMap">
        ///   A dictionary of tags to query, mapping from data source name to tag names.
        /// </param>
        /// <param name="utcStartTime">
        ///   The UTC start time for the query.
        /// </param>
        /// <param name="utcEndTime">
        ///   The UTC end time for the query.
        /// </param>
        /// <param name="dataFunction">
        ///   The data processing function to use. See <see cref="DataFunctions"/> for 
        ///   commonly-supported function names.
        /// </param>
        /// <param name="sampleInterval">
        ///   The sample interval to use when processing the tag values. For example, you may want 
        ///   to request the average value of a tag over the last day at a 1 hour sample interval.
        /// </param>
        /// <param name="properties">
        ///   Bespoke query properties.
        /// </param>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, 
        ///   this will be passed to the handler's callback when requesting the <c>Authorize</c> 
        ///   header value for the outgoing request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The historical tag values, indexed by data source name and then tag name.
        /// </returns>
        public static Task<IDictionary<string, HistoricalTagValuesDictionary>> ReadProcessedTagValuesAsync<TContext, TOptions>(
            this DataSourcesClient<TContext, TOptions> client,
            IDictionary<string, string[]> tagMap,
            DateTime utcStartTime,
            DateTime utcEndTime,
            string dataFunction,
            TimeSpan sampleInterval,
            IDictionary<string, string> properties = null,
            TContext context = default,
            CancellationToken cancellationToken = default
        ) where TOptions : DataCoreHttpClientOptions {

            if (client == null) {
                throw new ArgumentNullException(nameof(client));
            }

            return client.ReadProcessedTagValuesAsync(
                new ReadProcessedTagValuesRequest() {
                    Tags = tagMap,
                    StartTime = utcStartTime,
                    EndTime = utcEndTime,
                    DataFunction = dataFunction,
                    SampleInterval = sampleInterval,
                    QueryProperties = properties
                },
                context,
                cancellationToken
            );
        }


        /// <summary>
        /// Gets processed (aggregated) historical tag values.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The context type that is passed to API calls to allow authentication headers to be added 
        ///   to outgoing requests.
        /// </typeparam>
        /// <typeparam name="TOptions">
        ///   The options type for the client.
        /// </typeparam>
        /// <param name="client">
        ///   The Data Core client.
        /// </param>
        /// <param name="tagMap">
        ///   A dictionary of tags to query, mapping from data source name to tag names.
        /// </param>
        /// <param name="utcStartTime">
        ///   The UTC start time for the query.
        /// </param>
        /// <param name="utcEndTime">
        ///   The UTC end time for the query.
        /// </param>
        /// <param name="dataFunction">
        ///   The data processing function to use. See <see cref="DataFunctions"/> for 
        ///   commonly-supported function names.
        /// </param>
        /// <param name="sampleInterval">
        ///   The sample interval to use when processing the tag values.
        /// </param>
        /// <param name="properties">
        ///   Bespoke query properties.
        /// </param>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, 
        ///   this will be passed to the handler's callback when requesting the <c>Authorize</c> 
        ///   header value for the outgoing request.
        /// </param>
        /// <param name="cultureInfo">
        ///   The <see cref="CultureInfo"/> to use in the sample interval conversion.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The historical tag values, indexed by data source name and then tag name.
        /// </returns>
        public static Task<IDictionary<string, HistoricalTagValuesDictionary>> ReadProcessedTagValuesAsync<TContext, TOptions>(
            this DataSourcesClient<TContext, TOptions> client,
            IDictionary<string, string[]> tagMap,
            DateTime utcStartTime,
            DateTime utcEndTime,
            string dataFunction,
            string sampleInterval,
            IDictionary<string, string> properties = null,
            TContext context = default,
            CultureInfo cultureInfo = null,
            CancellationToken cancellationToken = default
        ) where TOptions : DataCoreHttpClientOptions {

            ParseSampleInterval(sampleInterval, cultureInfo, out var ts);

            return ReadProcessedTagValuesAsync(
                client,
                tagMap,
                utcStartTime,
                utcEndTime,
                dataFunction,
                ts,
                properties,
                context,
                cancellationToken
            );
        }


        /// <summary>
        /// Gets processed (aggregated) historical tag values.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The context type that is passed to API calls to allow authentication headers to be added 
        ///   to outgoing requests.
        /// </typeparam>
        /// <typeparam name="TOptions">
        ///   The options type for the client.
        /// </typeparam>
        /// <param name="client">
        ///   The Data Core client.
        /// </param>
        /// <param name="tagMap">
        ///   A dictionary of tags to query, mapping from data source name to tag names.
        /// </param>
        /// <param name="absoluteOrRelativeStartTime">
        ///   The absolute or relative start time for the query.
        /// </param>
        /// <param name="absoluteOrRelativeEndTime">
        ///   The absolute or relative end time for the query.
        /// </param>
        /// <param name="dataFunction">
        ///   The data processing function to use. See <see cref="DataFunctions"/> for 
        ///   commonly-supported function names.
        /// </param>
        /// <param name="sampleInterval">
        ///   The sample interval to use when processing the tag values. For example, you may want 
        ///   to request the average value of a tag over the last day at a 1 hour sample interval.
        /// </param>
        /// <param name="properties">
        ///   Bespoke query properties.
        /// </param>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, 
        ///   this will be passed to the handler's callback when requesting the <c>Authorize</c> 
        ///   header value for the outgoing request.
        /// </param>
        /// <param name="cultureInfo">
        ///   The <see cref="CultureInfo"/> to use in relative timestamp conversions.
        /// </param>
        /// <param name="timeZone">
        ///   The assumed time zone in relative timestamp conversions. Specify <see langword="null"/>
        ///   to assume UTC.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The historical tag values, indexed by data source name and then tag name.
        /// </returns>
        public static Task<IDictionary<string, HistoricalTagValuesDictionary>> ReadProcessedTagValuesAsync<TContext, TOptions>(
            this DataSourcesClient<TContext, TOptions> client,
            IDictionary<string, string[]> tagMap,
            string absoluteOrRelativeStartTime,
            string absoluteOrRelativeEndTime,
            string dataFunction,
            TimeSpan sampleInterval,
            IDictionary<string, string> properties = null,
            TContext context = default,
            CultureInfo cultureInfo = null,
            TimeZoneInfo timeZone = null,
            CancellationToken cancellationToken = default
        ) where TOptions : DataCoreHttpClientOptions {

            ParseTimeStamp(absoluteOrRelativeStartTime, cultureInfo, timeZone, nameof(absoluteOrRelativeStartTime), out var utcStartTime);
            ParseTimeStamp(absoluteOrRelativeEndTime, cultureInfo, timeZone, nameof(absoluteOrRelativeEndTime), out var utcEndTime);

            return ReadProcessedTagValuesAsync(
                client,
                tagMap,
                utcStartTime,
                utcEndTime,
                dataFunction,
                sampleInterval,
                properties,
                context,
                cancellationToken
            );
        }


        /// <summary>
        /// Gets processed (aggregated) historical tag values.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The context type that is passed to API calls to allow authentication headers to be added 
        ///   to outgoing requests.
        /// </typeparam>
        /// <typeparam name="TOptions">
        ///   The options type for the client.
        /// </typeparam>
        /// <param name="client">
        ///   The Data Core client.
        /// </param>
        /// <param name="tagMap">
        ///   A dictionary of tags to query, mapping from data source name to tag names.
        /// </param>
        /// <param name="absoluteOrRelativeStartTime">
        ///   The absolute or relative start time for the query.
        /// </param>
        /// <param name="absoluteOrRelativeEndTime">
        ///   The absolute or relative end time for the query.
        /// </param>
        /// <param name="dataFunction">
        ///   The data processing function to use. See <see cref="DataFunctions"/> for 
        ///   commonly-supported function names.
        /// </param>
        /// <param name="sampleInterval">
        ///   The sample interval to use when processing the tag values.
        /// </param>
        /// <param name="properties">
        ///   Bespoke query properties.
        /// </param>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, 
        ///   this will be passed to the handler's callback when requesting the <c>Authorize</c> 
        ///   header value for the outgoing request.
        /// </param>
        /// <param name="cultureInfo">
        ///   The <see cref="CultureInfo"/> to use in relative timestamp and sample interval conversions.
        /// </param>
        /// <param name="timeZone">
        ///   The assumed time zone in relative timestamp conversions. Specify <see langword="null"/>
        ///   to assume UTC.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The historical tag values, indexed by data source name and then tag name.
        /// </returns>
        public static Task<IDictionary<string, HistoricalTagValuesDictionary>> ReadProcessedTagValuesAsync<TContext, TOptions>(
            this DataSourcesClient<TContext, TOptions> client,
            IDictionary<string, string[]> tagMap,
            string absoluteOrRelativeStartTime,
            string absoluteOrRelativeEndTime,
            string dataFunction,
            string sampleInterval,
            IDictionary<string, string> properties = null,
            TContext context = default,
            CultureInfo cultureInfo = null,
            TimeZoneInfo timeZone = null,
            CancellationToken cancellationToken = default
        ) where TOptions : DataCoreHttpClientOptions {

            ParseTimeStamp(absoluteOrRelativeStartTime, cultureInfo, timeZone, nameof(absoluteOrRelativeStartTime), out var utcStartTime);
            ParseTimeStamp(absoluteOrRelativeEndTime, cultureInfo, timeZone, nameof(absoluteOrRelativeEndTime), out var utcEndTime);

            return ReadProcessedTagValuesAsync(
                client,
                tagMap,
                utcStartTime,
                utcEndTime,
                dataFunction,
                sampleInterval,
                properties,
                context,
                cultureInfo,
                cancellationToken
            );
        }


        /// <summary>
        /// Gets processed (aggregated) historical tag values.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The context type that is passed to API calls to allow authentication headers to be added 
        ///   to outgoing requests.
        /// </typeparam>
        /// <typeparam name="TOptions">
        ///   The options type for the client.
        /// </typeparam>
        /// <param name="client">
        ///   The Data Core client.
        /// </param>
        /// <param name="dataSourceName">
        ///   The data source to query.
        /// </param>
        /// <param name="tagNames">
        ///   The tags to query.
        /// </param>
        /// <param name="utcStartTime">
        ///   The UTC start time for the query.
        /// </param>
        /// <param name="utcEndTime">
        ///   The UTC end time for the query.
        /// </param>
        /// <param name="dataFunction">
        ///   The data processing function to use. See <see cref="DataFunctions"/> for 
        ///   commonly-supported function names.
        /// </param>
        /// <param name="sampleInterval">
        ///   The sample interval to use when processing the tag values. For example, you may want 
        ///   to request the average value of a tag over the last day at a 1 hour sample interval.
        /// </param>
        /// <param name="properties">
        ///   Bespoke query properties.
        /// </param>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, 
        ///   this will be passed to the handler's callback when requesting the <c>Authorize</c> 
        ///   header value for the outgoing request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The historical tag values, indexed by data source name and then tag name.
        /// </returns>
        public static async Task<HistoricalTagValuesDictionary> ReadProcessedTagValuesAsync<TContext, TOptions>(
            this DataSourcesClient<TContext, TOptions> client,
            string dataSourceName,
            IEnumerable<string> tagNames,
            DateTime utcStartTime,
            DateTime utcEndTime,
            string dataFunction,
            TimeSpan sampleInterval,
            IDictionary<string, string> properties = null,
            TContext context = default,
            CancellationToken cancellationToken = default
        ) where TOptions : DataCoreHttpClientOptions {

            if (string.IsNullOrWhiteSpace(dataSourceName)) {
                throw new ArgumentException(Resources.Error_DataSourceNameIsRequired, nameof(dataSourceName));
            }

            var result = await ReadProcessedTagValuesAsync(
                client,
                new Dictionary<string, string[]>() {
                    { dataSourceName, tagNames?.Distinct()?.ToArray() }
                },
                utcStartTime,
                utcEndTime,
                dataFunction,
                sampleInterval,
                properties,
                context,
                cancellationToken
            ).ConfigureAwait(false);

            return result?.FirstOrDefault().Value;
        }


        /// <summary>
        /// Gets processed (aggregated) historical tag values.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The context type that is passed to API calls to allow authentication headers to be added 
        ///   to outgoing requests.
        /// </typeparam>
        /// <typeparam name="TOptions">
        ///   The options type for the client.
        /// </typeparam>
        /// <param name="client">
        ///   The Data Core client.
        /// </param>
        /// <param name="dataSourceName">
        ///   The data source to query.
        /// </param>
        /// <param name="tagNames">
        ///   The tags to query.
        /// </param>
        /// <param name="utcStartTime">
        ///   The UTC start time for the query.
        /// </param>
        /// <param name="utcEndTime">
        ///   The UTC end time for the query.
        /// </param>
        /// <param name="dataFunction">
        ///   The data processing function to use. See <see cref="DataFunctions"/> for 
        ///   commonly-supported function names.
        /// </param>
        /// <param name="sampleInterval">
        ///   The sample interval to use when processing the tag values.
        /// </param>
        /// <param name="properties">
        ///   Bespoke query properties.
        /// </param>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, 
        ///   this will be passed to the handler's callback when requesting the <c>Authorize</c> 
        ///   header value for the outgoing request.
        /// </param>
        /// <param name="cultureInfo">
        ///   The <see cref="CultureInfo"/> to use in the sample interval conversion.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The historical tag values, indexed by data source name and then tag name.
        /// </returns>
        public static Task<HistoricalTagValuesDictionary> ReadProcessedTagValuesAsync<TContext, TOptions>(
            this DataSourcesClient<TContext, TOptions> client,
            string dataSourceName,
            IEnumerable<string> tagNames,
            DateTime utcStartTime,
            DateTime utcEndTime,
            string dataFunction,
            string sampleInterval,
            IDictionary<string, string> properties = null,
            TContext context = default,
            CultureInfo cultureInfo = null,
            CancellationToken cancellationToken = default
        ) where TOptions : DataCoreHttpClientOptions {

            ParseSampleInterval(sampleInterval, cultureInfo, out var ts);

            return ReadProcessedTagValuesAsync(
                client,
                dataSourceName,
                tagNames,
                utcStartTime,
                utcEndTime,
                dataFunction,
                ts,
                properties,
                context,
                cancellationToken
            );
        }


        /// <summary>
        /// Gets processed (aggregated) historical tag values.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The context type that is passed to API calls to allow authentication headers to be added 
        ///   to outgoing requests.
        /// </typeparam>
        /// <typeparam name="TOptions">
        ///   The options type for the client.
        /// </typeparam>
        /// <param name="client">
        ///   The Data Core client.
        /// </param>
        /// <param name="dataSourceName">
        ///   The data source to query.
        /// </param>
        /// <param name="tagNames">
        ///   The tags to query.
        /// </param>
        /// <param name="absoluteOrRelativeStartTime">
        ///   The absolute or relative start time for the query.
        /// </param>
        /// <param name="absoluteOrRelativeEndTime">
        ///   The absolute or relative end time for the query.
        /// </param>
        /// <param name="dataFunction">
        ///   The data processing function to use. See <see cref="DataFunctions"/> for 
        ///   commonly-supported function names.
        /// </param>
        /// <param name="sampleInterval">
        ///   The sample interval to use when processing the tag values. For example, you may want 
        ///   to request the average value of a tag over the last day at a 1 hour sample interval.
        /// </param>
        /// <param name="properties">
        ///   Bespoke query properties.
        /// </param>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, 
        ///   this will be passed to the handler's callback when requesting the <c>Authorize</c> 
        ///   header value for the outgoing request.
        /// </param>
        /// <param name="cultureInfo">
        ///   The <see cref="CultureInfo"/> to use in relative timestamp conversions.
        /// </param>
        /// <param name="timeZone">
        ///   The assumed time zone in relative timestamp conversions. Specify <see langword="null"/>
        ///   to assume UTC.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The historical tag values, indexed by data source name and then tag name.
        /// </returns>
        public static Task<HistoricalTagValuesDictionary> ReadProcessedTagValuesAsync<TContext, TOptions>(
            this DataSourcesClient<TContext, TOptions> client,
            string dataSourceName,
            IEnumerable<string> tagNames,
            string absoluteOrRelativeStartTime,
            string absoluteOrRelativeEndTime,
            string dataFunction,
            TimeSpan sampleInterval,
            IDictionary<string, string> properties = null,
            TContext context = default,
            CultureInfo cultureInfo = null,
            TimeZoneInfo timeZone = null,
            CancellationToken cancellationToken = default
        ) where TOptions : DataCoreHttpClientOptions {

            ParseTimeStamp(absoluteOrRelativeStartTime, cultureInfo, timeZone, nameof(absoluteOrRelativeStartTime), out var utcStartTime);
            ParseTimeStamp(absoluteOrRelativeEndTime, cultureInfo, timeZone, nameof(absoluteOrRelativeEndTime), out var utcEndTime);

            return ReadProcessedTagValuesAsync(
                client,
                dataSourceName,
                tagNames,
                utcStartTime,
                utcEndTime,
                dataFunction,
                sampleInterval,
                properties,
                context,
                cancellationToken
            );
        }


        /// <summary>
        /// Gets processed (aggregated) historical tag values.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The context type that is passed to API calls to allow authentication headers to be added 
        ///   to outgoing requests.
        /// </typeparam>
        /// <typeparam name="TOptions">
        ///   The options type for the client.
        /// </typeparam>
        /// <param name="client">
        ///   The Data Core client.
        /// </param>
        /// <param name="dataSourceName">
        ///   The data source to query.
        /// </param>
        /// <param name="tagNames">
        ///   The tags to query.
        /// </param>
        /// <param name="absoluteOrRelativeStartTime">
        ///   The absolute or relative start time for the query.
        /// </param>
        /// <param name="absoluteOrRelativeEndTime">
        ///   The absolute or relative end time for the query.
        /// </param>
        /// <param name="dataFunction">
        ///   The data processing function to use. See <see cref="DataFunctions"/> for 
        ///   commonly-supported function names.
        /// </param>
        /// <param name="sampleInterval">
        ///   The sample interval to use when processing the tag values. For example, you may want 
        ///   to request the average value of a tag over the last day at a 1 hour sample interval.
        /// </param>
        /// <param name="properties">
        ///   Bespoke query properties.
        /// </param>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, 
        ///   this will be passed to the handler's callback when requesting the <c>Authorize</c> 
        ///   header value for the outgoing request.
        /// </param>
        /// <param name="cultureInfo">
        ///   The <see cref="CultureInfo"/> to use in relative timestamp and sample interval conversions.
        /// </param>
        /// <param name="timeZone">
        ///   The assumed time zone in relative timestamp conversions. Specify <see langword="null"/>
        ///   to assume UTC.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The historical tag values, indexed by data source name and then tag name.
        /// </returns>
        public static Task<HistoricalTagValuesDictionary> ReadProcessedTagValuesAsync<TContext, TOptions>(
            this DataSourcesClient<TContext, TOptions> client,
            string dataSourceName,
            IEnumerable<string> tagNames,
            string absoluteOrRelativeStartTime,
            string absoluteOrRelativeEndTime,
            string dataFunction,
            string sampleInterval,
            IDictionary<string, string> properties = null,
            TContext context = default,
            CultureInfo cultureInfo = null,
            TimeZoneInfo timeZone = null,
            CancellationToken cancellationToken = default
        ) where TOptions : DataCoreHttpClientOptions {

            ParseSampleInterval(sampleInterval, cultureInfo, out var ts);

            return ReadProcessedTagValuesAsync(
                client,
                dataSourceName,
                tagNames,
                absoluteOrRelativeStartTime,
                absoluteOrRelativeEndTime,
                dataFunction,
                ts,
                properties,
                context,
                cultureInfo,
                timeZone,
                cancellationToken
            );
        }

        #endregion

        #region [ Tag Values At Times Queries ]

        /// <summary>
        /// Gets historical tag values at specific sample times.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The context type that is passed to API calls to allow authentication headers to be added 
        ///   to outgoing requests.
        /// </typeparam>
        /// <typeparam name="TOptions">
        ///   The options type for the client.
        /// </typeparam>
        /// <param name="client">
        ///   The Data Core client.
        /// </param>
        /// <param name="tagMap">
        ///   A dictionary of tags to query, mapping from data source name to tag names.
        /// </param>
        /// <param name="utcSampleTimes">
        ///   The UTC sample times to request values at.
        /// </param>
        /// <param name="properties">
        ///   Bespoke query properties.
        /// </param>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, 
        ///   this will be passed to the handler's callback when requesting the <c>Authorize</c> 
        ///   header value for the outgoing request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The historical tag values, indexed by data source name and then tag name.
        /// </returns>
        public static Task<IDictionary<string, HistoricalTagValuesDictionary>> ReadTagValuesAtTimesAsync<TContext, TOptions>(
            this DataSourcesClient<TContext, TOptions> client, 
            IDictionary<string, string[]> tagMap, 
            IEnumerable<DateTime> utcSampleTimes, 
            IDictionary<string, string> properties = null,
            TContext context = default,
            CancellationToken cancellationToken = default
        ) where TOptions : DataCoreHttpClientOptions {

            if (client == null) {
                throw new ArgumentNullException(nameof(client));
            }

            return client.ReadTagValuesAtTimesAsync(
                new ReadTagValuesAtTimesRequest() {
                    Tags = tagMap,
                    UtcSampleTimes = utcSampleTimes?.Distinct()?.ToArray(),
                    QueryProperties = properties
                },
                context,
                cancellationToken
            );
        }


        /// <summary>
        /// Gets historical tag values at specific sample times.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The context type that is passed to API calls to allow authentication headers to be added 
        ///   to outgoing requests.
        /// </typeparam>
        /// <typeparam name="TOptions">
        ///   The options type for the client.
        /// </typeparam>
        /// <param name="client">
        ///   The Data Core client.
        /// </param>
        /// <param name="tagMap">
        ///   A dictionary of tags to query, mapping from data source name to tag names.
        /// </param>
        /// <param name="absoluteOrRelativeSampleTimes">
        ///   The sample times to request values at.
        /// </param>
        /// <param name="properties">
        ///   Bespoke query properties.
        /// </param>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, 
        ///   this will be passed to the handler's callback when requesting the <c>Authorize</c> 
        ///   header value for the outgoing request.
        /// </param>
        /// <param name="cultureInfo">
        ///   The <see cref="CultureInfo"/> to use in relative timestamp conversions.
        /// </param>
        /// <param name="timeZone">
        ///   The assumed time zone in relative timestamp conversions. Specify <see langword="null"/>
        ///   to assume UTC.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The historical tag values, indexed by data source name and then tag name.
        /// </returns>
        public static Task<IDictionary<string, HistoricalTagValuesDictionary>> ReadTagValuesAtTimesAsync<TContext, TOptions>(
            this DataSourcesClient<TContext, TOptions> client,
            IDictionary<string, string[]> tagMap,
            IEnumerable<string> absoluteOrRelativeSampleTimes,
            IDictionary<string, string> properties = null,
            TContext context = default,
            CultureInfo cultureInfo = null,
            TimeZoneInfo timeZone = null,
            CancellationToken cancellationToken = default
        ) where TOptions : DataCoreHttpClientOptions {

            if (client == null) {
                throw new ArgumentNullException(nameof(client));
            }

            var sampleTimes = new List<DateTime>();
            if (absoluteOrRelativeSampleTimes != null) {
                foreach (var item in absoluteOrRelativeSampleTimes) {
                    ParseTimeStamp(item, cultureInfo, timeZone, nameof(absoluteOrRelativeSampleTimes), out var dt);
                    sampleTimes.Add(dt);
                }
            }

            return client.ReadTagValuesAtTimesAsync(
                new ReadTagValuesAtTimesRequest() {
                    Tags = tagMap,
                    UtcSampleTimes = sampleTimes.Distinct().ToArray(),
                    QueryProperties = properties
                },
                context,
                cancellationToken
            );
        }


        /// <summary>
        /// Gets historical tag values at specific sample times.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The context type that is passed to API calls to allow authentication headers to be added 
        ///   to outgoing requests.
        /// </typeparam>
        /// <typeparam name="TOptions">
        ///   The options type for the client.
        /// </typeparam>
        /// <param name="client">
        ///   The Data Core client.
        /// </param>
        /// <param name="dataSourceName">
        ///   The data source to query.
        /// </param>
        /// <param name="tagNames">
        ///   The tags to query.
        /// </param>
        /// <param name="utcSampleTimes">
        ///   The UTC sample times to request values at.
        /// </param>
        /// <param name="properties">
        ///   Bespoke query properties.
        /// </param>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, 
        ///   this will be passed to the handler's callback when requesting the <c>Authorize</c> 
        ///   header value for the outgoing request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The historical tag values, indexed by data source name and then tag name.
        /// </returns>
        public static async Task<HistoricalTagValuesDictionary> ReadTagValuesAtTimesAsync<TContext, TOptions>(
            this DataSourcesClient<TContext, TOptions> client,
            string dataSourceName,
            IEnumerable<string> tagNames,
            IEnumerable<DateTime> utcSampleTimes,
            IDictionary<string, string> properties = null,
            TContext context = default,
            CancellationToken cancellationToken = default
        ) where TOptions : DataCoreHttpClientOptions {

            if (string.IsNullOrWhiteSpace(dataSourceName)) {
                throw new ArgumentException(Resources.Error_DataSourceNameIsRequired, nameof(dataSourceName));
            }

            var result = await ReadTagValuesAtTimesAsync(
                client,
                new Dictionary<string, string[]>() {
                    { dataSourceName, tagNames?.Distinct()?.ToArray() }
                },
                utcSampleTimes,
                properties,
                context,
                cancellationToken
            ).ConfigureAwait(false);

            return result?.FirstOrDefault().Value;
        }


        /// <summary>
        /// Gets historical tag values at specific sample times.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The context type that is passed to API calls to allow authentication headers to be added 
        ///   to outgoing requests.
        /// </typeparam>
        /// <typeparam name="TOptions">
        ///   The options type for the client.
        /// </typeparam>
        /// <param name="client">
        ///   The Data Core client.
        /// </param>
        /// <param name="dataSourceName">
        ///   The data source to query.
        /// </param>
        /// <param name="tagNames">
        ///   The tags to query.
        /// </param>
        /// <param name="absoluteOrRelativeSampleTimes">
        ///   The sample times to request values at.
        /// </param>
        /// <param name="properties">
        ///   Bespoke query properties.
        /// </param>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, 
        ///   this will be passed to the handler's callback when requesting the <c>Authorize</c> 
        ///   header value for the outgoing request.
        /// </param>
        /// <param name="cultureInfo">
        ///   The <see cref="CultureInfo"/> to use in relative timestamp conversions.
        /// </param>
        /// <param name="timeZone">
        ///   The assumed time zone in relative timestamp conversions. Specify <see langword="null"/>
        ///   to assume UTC.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The historical tag values, indexed by data source name and then tag name.
        /// </returns>
        public static async Task<HistoricalTagValuesDictionary> ReadTagValuesAtTimesAsync<TContext, TOptions>(
            this DataSourcesClient<TContext, TOptions> client,
            string dataSourceName,
            IEnumerable<string> tagNames,
            IEnumerable<string> absoluteOrRelativeSampleTimes,
            IDictionary<string, string> properties = null,
            TContext context = default,
            CultureInfo cultureInfo = null,
            TimeZoneInfo timeZone = null,
            CancellationToken cancellationToken = default
        ) where TOptions : DataCoreHttpClientOptions {

            if (string.IsNullOrWhiteSpace(dataSourceName)) {
                throw new ArgumentException(Resources.Error_DataSourceNameIsRequired, nameof(dataSourceName));
            }

            var sampleTimes = new List<DateTime>();
            if (absoluteOrRelativeSampleTimes != null) {
                foreach (var item in absoluteOrRelativeSampleTimes) {
                    ParseTimeStamp(item, cultureInfo, timeZone, nameof(absoluteOrRelativeSampleTimes), out var dt);
                    sampleTimes.Add(dt);
                }
            }

            var result = await ReadTagValuesAtTimesAsync(
                client,
                new Dictionary<string, string[]>() {
                    { dataSourceName, tagNames?.Distinct()?.ToArray() }
                },
                sampleTimes.Distinct().ToArray(),
                properties,
                context,
                cancellationToken
            ).ConfigureAwait(false);

            return result?.FirstOrDefault().Value;
        }

        #endregion

        #region [ Write Tag Value Operations ]

        /// <summary>
        /// Writes values to a data source's snapshot.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The context type that is passed to API calls to allow authentication headers to be added 
        ///   to outgoing requests.
        /// </typeparam>
        /// <typeparam name="TOptions">
        ///   The options type for the client.
        /// </typeparam>
        /// <param name="client">
        ///   The client.
        /// </param>
        /// <param name="dataSourceName">
        ///   The data source to write to.
        /// </param>
        /// <param name="values">
        ///   The values to write.
        /// </param>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, 
        ///   this will be passed to the handler's callback when requesting the <c>Authorize</c> 
        ///   header value for the outgoing request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A <see cref="TagValueUpdateResponse"/> describing the write results for each tag 
        ///   that was written to.
        /// </returns>
        public static Task<IEnumerable<TagValueUpdateResponse>> WriteSnapshotTagValuesAsync<TContext, TOptions>(
            this DataSourcesClient<TContext, TOptions> client,
            string dataSourceName,
            IEnumerable<TagValue> values,
            TContext context = default,
            CancellationToken cancellationToken = default
        ) where TOptions : DataCoreHttpClientOptions {
            if (client == null) {
                throw new ArgumentNullException(nameof(client));
            }

            if (string.IsNullOrWhiteSpace(dataSourceName)) {
                throw new ArgumentException(Resources.Error_DataSourceNameIsRequired, nameof(dataSourceName));
            }

            return client.WriteSnapshotTagValuesAsync(new WriteTagValuesRequest() { 
                DataSourceName = dataSourceName,
                Values = values?.Where(x => x != null).ToArray()
            }, context, cancellationToken);
        }


        /// <summary>
        /// Writes numeric values to a single tag in a data source's snapshot.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The context type that is passed to API calls to allow authentication headers to be added 
        ///   to outgoing requests.
        /// </typeparam>
        /// <typeparam name="TOptions">
        ///   The options type for the client.
        /// </typeparam>
        /// <param name="client">
        ///   The client.
        /// </param>
        /// <param name="dataSourceName">
        ///   The data source to write to.
        /// </param>
        /// <param name="tagName">
        ///   The tag to write to.
        /// </param>
        /// <param name="values">
        ///   The values to write.
        /// </param>
        /// <param name="status">
        ///   The quality status for the values.
        /// </param>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, 
        ///   this will be passed to the handler's callback when requesting the <c>Authorize</c> 
        ///   header value for the outgoing request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A <see cref="TagValueUpdateResponse"/> describing the write results for the tag.
        /// </returns>
        public static async Task<TagValueUpdateResponse> WriteSnapshotTagValuesAsync<TContext, TOptions>(
            this DataSourcesClient<TContext, TOptions> client,
            string dataSourceName,
            string tagName,
            IDictionary<DateTime, double> values,
            TagValueStatus status = TagValueStatus.Good,
            TContext context = default,
            CancellationToken cancellationToken = default
        ) where TOptions : DataCoreHttpClientOptions {
            if (client == null) {
                throw new ArgumentNullException(nameof(client));
            }

            if (string.IsNullOrWhiteSpace(dataSourceName)) {
                throw new ArgumentException(Resources.Error_DataSourceNameIsRequired, nameof(dataSourceName));
            }

            if (string.IsNullOrWhiteSpace(tagName)) {
                throw new ArgumentException(Resources.Error_TagNameIsRequired, nameof(tagName));
            }

            var result = await WriteSnapshotTagValuesAsync(
                client,
                dataSourceName,
                values?.OrderBy(x => x.Key).Select(x => new TagValue(tagName, x.Key, x.Value, null, status, null)),
                context,
                cancellationToken
            ).ConfigureAwait(false);

            return result?.FirstOrDefault();
        }


        /// <summary>
        /// Writes text values to a single tag in a data source's snapshot.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The context type that is passed to API calls to allow authentication headers to be added 
        ///   to outgoing requests.
        /// </typeparam>
        /// <typeparam name="TOptions">
        ///   The options type for the client.
        /// </typeparam>
        /// <param name="client">
        ///   The client.
        /// </param>
        /// <param name="dataSourceName">
        ///   The data source to write to.
        /// </param>
        /// <param name="tagName">
        ///   The tag to write to.
        /// </param>
        /// <param name="values">
        ///   The values to write.
        /// </param>
        /// <param name="status">
        ///   The quality status for the values.
        /// </param>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, 
        ///   this will be passed to the handler's callback when requesting the <c>Authorize</c> 
        ///   header value for the outgoing request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A <see cref="TagValueUpdateResponse"/> describing the write results for the tag.
        /// </returns>
        public static async Task<TagValueUpdateResponse> WriteSnapshotTagValuesAsync<TContext, TOptions>(
            this DataSourcesClient<TContext, TOptions> client,
            string dataSourceName,
            string tagName,
            IDictionary<DateTime, string> values,
            TagValueStatus status = TagValueStatus.Good,
            TContext context = default,
            CancellationToken cancellationToken = default
        ) where TOptions : DataCoreHttpClientOptions {
            if (client == null) {
                throw new ArgumentNullException(nameof(client));
            }

            if (string.IsNullOrWhiteSpace(dataSourceName)) {
                throw new ArgumentException(Resources.Error_DataSourceNameIsRequired, nameof(dataSourceName));
            }

            if (string.IsNullOrWhiteSpace(tagName)) {
                throw new ArgumentException(Resources.Error_TagNameIsRequired, nameof(tagName));
            }

            var result = await WriteSnapshotTagValuesAsync(
                client,
                dataSourceName,
                values?.OrderBy(x => x.Key).Select(x => new TagValue(tagName, x.Key, double.NaN, x.Value, status, null)),
                context,
                cancellationToken
            ).ConfigureAwait(false);

            return result?.FirstOrDefault();
        }


        /// <summary>
        /// Writes a single numeric value to a single tag in a data source's snapshot.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The context type that is passed to API calls to allow authentication headers to be added 
        ///   to outgoing requests.
        /// </typeparam>
        /// <typeparam name="TOptions">
        ///   The options type for the client.
        /// </typeparam>
        /// <param name="client">
        ///   The client.
        /// </param>
        /// <param name="dataSourceName">
        ///   The data source to write to.
        /// </param>
        /// <param name="tagName">
        ///   The tag to write to.
        /// </param>
        /// <param name="utcSampleTime">
        ///   The UTC sample time of the value.
        /// </param>
        /// <param name="value">
        ///   The value to write.
        /// </param>
        /// <param name="status">
        ///   The quality status for the value.
        /// </param>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, 
        ///   this will be passed to the handler's callback when requesting the <c>Authorize</c> 
        ///   header value for the outgoing request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A <see cref="TagValueUpdateResponse"/> describing the write results for the tag.
        /// </returns>
        public static async Task<TagValueUpdateResponse> WriteSnapshotTagValueAsync<TContext, TOptions>(
            this DataSourcesClient<TContext, TOptions> client,
            string dataSourceName,
            string tagName,
            DateTime utcSampleTime,
            double value,
            TagValueStatus status = TagValueStatus.Good,
            TContext context = default,
            CancellationToken cancellationToken = default
        ) where TOptions : DataCoreHttpClientOptions {
            if (client == null) {
                throw new ArgumentNullException(nameof(client));
            }

            if (string.IsNullOrWhiteSpace(dataSourceName)) {
                throw new ArgumentException(Resources.Error_DataSourceNameIsRequired, nameof(dataSourceName));
            }

            if (string.IsNullOrWhiteSpace(tagName)) {
                throw new ArgumentException(Resources.Error_TagNameIsRequired, nameof(tagName));
            }

            var result = await WriteSnapshotTagValuesAsync(
                client,
                dataSourceName,
                new[] {
                    new TagValue(
                        tagName, 
                        utcSampleTime, 
                        value, 
                        null, 
                        status, 
                        null
                    )
                },
                context,
                cancellationToken
            ).ConfigureAwait(false);

            return result?.FirstOrDefault();
        }


        /// <summary>
        /// Writes a single text value to a single tag in a data source's snapshot.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The context type that is passed to API calls to allow authentication headers to be added 
        ///   to outgoing requests.
        /// </typeparam>
        /// <typeparam name="TOptions">
        ///   The options type for the client.
        /// </typeparam>
        /// <param name="client">
        ///   The client.
        /// </param>
        /// <param name="dataSourceName">
        ///   The data source to write to.
        /// </param>
        /// <param name="tagName">
        ///   The tag to write to.
        /// </param>
        /// <param name="utcSampleTime">
        ///   The UTC sample time of the value.
        /// </param>
        /// <param name="value">
        ///   The value to write.
        /// </param>
        /// <param name="status">
        ///   The quality status for the value.
        /// </param>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, 
        ///   this will be passed to the handler's callback when requesting the <c>Authorize</c> 
        ///   header value for the outgoing request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A <see cref="TagValueUpdateResponse"/> describing the write results for the tag.
        /// </returns>
        public static async Task<TagValueUpdateResponse> WriteSnapshotTagValueAsync<TContext, TOptions>(
            this DataSourcesClient<TContext, TOptions> client,
            string dataSourceName,
            string tagName,
            DateTime utcSampleTime,
            string value,
            TagValueStatus status = TagValueStatus.Good,
            TContext context = default,
            CancellationToken cancellationToken = default
        ) where TOptions : DataCoreHttpClientOptions {
            if (client == null) {
                throw new ArgumentNullException(nameof(client));
            }

            if (string.IsNullOrWhiteSpace(dataSourceName)) {
                throw new ArgumentException(Resources.Error_DataSourceNameIsRequired, nameof(dataSourceName));
            }

            if (string.IsNullOrWhiteSpace(tagName)) {
                throw new ArgumentException(Resources.Error_TagNameIsRequired, nameof(tagName));
            }

            var result = await WriteSnapshotTagValuesAsync(
                client,
                dataSourceName,
                new[] {
                    new TagValue(
                        tagName,
                        utcSampleTime,
                        double.NaN,
                        value,
                        status,
                        null
                    )
                },
                context,
                cancellationToken
            ).ConfigureAwait(false);

            return result?.FirstOrDefault();
        }


        /// <summary>
        /// Writes values to a data source's history archive.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The context type that is passed to API calls to allow authentication headers to be added 
        ///   to outgoing requests.
        /// </typeparam>
        /// <typeparam name="TOptions">
        ///   The options type for the client.
        /// </typeparam>
        /// <param name="client">
        ///   The client.
        /// </param>
        /// <param name="dataSourceName">
        ///   The data source to write to.
        /// </param>
        /// <param name="values">
        ///   The values to write.
        /// </param>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, 
        ///   this will be passed to the handler's callback when requesting the <c>Authorize</c> 
        ///   header value for the outgoing request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A <see cref="TagValueUpdateResponse"/> describing the write results for each tag 
        ///   that was written to.
        /// </returns>
        public static Task<IEnumerable<TagValueUpdateResponse>> WriteHistoricalTagValuesAsync<TContext, TOptions>(
            this DataSourcesClient<TContext, TOptions> client,
            string dataSourceName,
            IEnumerable<TagValue> values,
            TContext context = default,
            CancellationToken cancellationToken = default
        ) where TOptions : DataCoreHttpClientOptions {
            if (client == null) {
                throw new ArgumentNullException(nameof(client));
            }

            if (string.IsNullOrWhiteSpace(dataSourceName)) {
                throw new ArgumentException(Resources.Error_DataSourceNameIsRequired, nameof(dataSourceName));
            }

            return client.WriteHistoricalTagValuesAsync(new WriteTagValuesRequest() {
                DataSourceName = dataSourceName,
                Values = values?.Where(x => x != null).ToArray()
            }, context, cancellationToken);
        }


        /// <summary>
        /// Writes numeric values to a single tag in a data source's history archive.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The context type that is passed to API calls to allow authentication headers to be added 
        ///   to outgoing requests.
        /// </typeparam>
        /// <typeparam name="TOptions">
        ///   The options type for the client.
        /// </typeparam>
        /// <param name="client">
        ///   The client.
        /// </param>
        /// <param name="dataSourceName">
        ///   The data source to write to.
        /// </param>
        /// <param name="tagName">
        ///   The tag to write to.
        /// </param>
        /// <param name="values">
        ///   The values to write.
        /// </param>
        /// <param name="status">
        ///   The quality status for the values.
        /// </param>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, 
        ///   this will be passed to the handler's callback when requesting the <c>Authorize</c> 
        ///   header value for the outgoing request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A <see cref="TagValueUpdateResponse"/> describing the write results for each tag 
        ///   that was written to.
        /// </returns>
        public static async Task<TagValueUpdateResponse> WriteHistoricalTagValuesAsync<TContext, TOptions>(
            this DataSourcesClient<TContext, TOptions> client,
            string dataSourceName,
            string tagName,
            IDictionary<DateTime, double> values,
            TagValueStatus status = TagValueStatus.Good,
            TContext context = default,
            CancellationToken cancellationToken = default
        ) where TOptions : DataCoreHttpClientOptions {
            if (client == null) {
                throw new ArgumentNullException(nameof(client));
            }

            if (string.IsNullOrWhiteSpace(dataSourceName)) {
                throw new ArgumentException(Resources.Error_DataSourceNameIsRequired, nameof(dataSourceName));
            }

            if (string.IsNullOrWhiteSpace(tagName)) {
                throw new ArgumentException(Resources.Error_TagNameIsRequired, nameof(tagName));
            }

            var result = await WriteHistoricalTagValuesAsync(
                client,
                dataSourceName,
                values?.OrderBy(x => x.Key).Select(x => new TagValue(tagName, x.Key, x.Value, null, status, null)),
                context,
                cancellationToken
            ).ConfigureAwait(false);

            return result?.FirstOrDefault();
        }


        /// <summary>
        /// Writes text values to a single tag in a data source's history archive.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The context type that is passed to API calls to allow authentication headers to be added 
        ///   to outgoing requests.
        /// </typeparam>
        /// <typeparam name="TOptions">
        ///   The options type for the client.
        /// </typeparam>
        /// <param name="client">
        ///   The client.
        /// </param>
        /// <param name="dataSourceName">
        ///   The data source to write to.
        /// </param>
        /// <param name="tagName">
        ///   The tag to write to.
        /// </param>
        /// <param name="values">
        ///   The values to write.
        /// </param>
        /// <param name="status">
        ///   The quality status for the values.
        /// </param>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, 
        ///   this will be passed to the handler's callback when requesting the <c>Authorize</c> 
        ///   header value for the outgoing request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A <see cref="TagValueUpdateResponse"/> describing the write results for each tag 
        ///   that was written to.
        /// </returns>
        public static async Task<TagValueUpdateResponse> WriteHistoricalTagValuesAsync<TContext, TOptions>(
            this DataSourcesClient<TContext, TOptions> client,
            string dataSourceName,
            string tagName,
            IDictionary<DateTime, string> values,
            TagValueStatus status = TagValueStatus.Good,
            TContext context = default,
            CancellationToken cancellationToken = default
        ) where TOptions : DataCoreHttpClientOptions {
            if (client == null) {
                throw new ArgumentNullException(nameof(client));
            }

            if (string.IsNullOrWhiteSpace(dataSourceName)) {
                throw new ArgumentException(Resources.Error_DataSourceNameIsRequired, nameof(dataSourceName));
            }

            if (string.IsNullOrWhiteSpace(tagName)) {
                throw new ArgumentException(Resources.Error_TagNameIsRequired, nameof(tagName));
            }

            var result = await WriteHistoricalTagValuesAsync(
                client,
                dataSourceName,
                values?.OrderBy(x => x.Key).Select(x => new TagValue(tagName, x.Key, double.NaN, x.Value, status, null)),
                context,
                cancellationToken
            ).ConfigureAwait(false);

            return result?.FirstOrDefault();
        }

        #endregion

        #region [ Annotation Queries ]

        /// <summary>
        /// Reads annotations from a data source.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The context type that is passed to API calls to allow authentication headers to be added 
        ///   to outgoing requests.
        /// </typeparam>
        /// <typeparam name="TOptions">
        ///   The options type for the client.
        /// </typeparam>
        /// <param name="client">
        ///   The client.
        /// </param>
        /// <param name="dataSourceName">
        ///   The data source name to query.
        /// </param>
        /// <param name="tagNames">
        ///   The tags to query.
        /// </param>
        /// <param name="utcStartTime">
        ///   The UTC start time for the query.
        /// </param>
        /// <param name="utcEndTime">
        ///   The UTC end time for the query.
        /// </param>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, 
        ///   this will be passed to the handler's callback when requesting the <c>Authorize</c> 
        ///   header value for the outgoing request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A collection of <see cref="AnnotationCollection"/> describing the annotations for 
        ///   each tag in the query.
        /// </returns>
        public static async Task<IEnumerable<AnnotationCollection>> ReadAnnotationsAsync<TContext, TOptions>(
            this DataSourcesClient<TContext, TOptions> client,
            string dataSourceName,
            IEnumerable<string> tagNames,
            DateTime utcStartTime,
            DateTime utcEndTime,
            TContext context = default,
            CancellationToken cancellationToken = default
        ) where TOptions : DataCoreHttpClientOptions {
            if (client == null) {
                throw new ArgumentNullException(nameof(client));
            }

            if (string.IsNullOrWhiteSpace(dataSourceName)) {
                throw new ArgumentException(Resources.Error_DataSourceNameIsRequired, nameof(dataSourceName));
            }

            var result = await client.ReadAnnotationsAsync(new ReadAnnotationsRequest() { 
                DataSourceName = dataSourceName,
                TagNames = tagNames?.ToArray(),
                StartTime = utcStartTime,
                EndTime = utcEndTime
            }, context, cancellationToken).ConfigureAwait(false);

            return result;
        }


        /// <summary>
        /// Reads annotations from a data source.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The context type that is passed to API calls to allow authentication headers to be added 
        ///   to outgoing requests.
        /// </typeparam>
        /// <typeparam name="TOptions">
        ///   The options type for the client.
        /// </typeparam>
        /// <param name="client">
        ///   The client.
        /// </param>
        /// <param name="dataSourceName">
        ///   The data source name to query.
        /// </param>
        /// <param name="tagNames">
        ///   The tags to query.
        /// </param>
        /// <param name="absoluteOrRelativeStartTime">
        ///   The absolute or relative start time for the query.
        /// </param>
        /// <param name="absoluteOrRelativeEndTime">
        ///   The absolute or relative end time for the query.
        /// </param>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, 
        ///   this will be passed to the handler's callback when requesting the <c>Authorize</c> 
        ///   header value for the outgoing request.
        /// </param>
        /// <param name="cultureInfo">
        ///   The <see cref="CultureInfo"/> to use in relative timestamp conversions.
        /// </param>
        /// <param name="timeZone">
        ///   The assumed time zone in relative timestamp conversions. Specify <see langword="null"/>
        ///   to assume UTC.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A collection of <see cref="AnnotationCollection"/> describing the annotations for 
        ///   each tag in the query.
        /// </returns>
        public static Task<IEnumerable<AnnotationCollection>> ReadAnnotationsAsync<TContext, TOptions>(
            this DataSourcesClient<TContext, TOptions> client,
            string dataSourceName,
            IEnumerable<string> tagNames,
            string absoluteOrRelativeStartTime,
            string absoluteOrRelativeEndTime,
            TContext context = default,
            CultureInfo cultureInfo = null,
            TimeZoneInfo timeZone = null,
            CancellationToken cancellationToken = default
        ) where TOptions : DataCoreHttpClientOptions {
            if (client == null) {
                throw new ArgumentNullException(nameof(client));
            }

            if (string.IsNullOrWhiteSpace(dataSourceName)) {
                throw new ArgumentException(Resources.Error_DataSourceNameIsRequired, nameof(dataSourceName));
            }

            ParseTimeStamp(absoluteOrRelativeStartTime, cultureInfo, timeZone, nameof(absoluteOrRelativeStartTime), out var utcStartTime);
            ParseTimeStamp(absoluteOrRelativeEndTime, cultureInfo, timeZone, nameof(absoluteOrRelativeEndTime), out var utcEndTime);

            return ReadAnnotationsAsync(
                client,
                dataSourceName,
                tagNames,
                utcStartTime,
                utcEndTime,
                context,
                cancellationToken
            );
        }

        #endregion

    }
}
