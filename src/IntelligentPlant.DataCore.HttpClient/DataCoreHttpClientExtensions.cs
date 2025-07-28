using IntelligentPlant.DataCore.Client.Clients;
using IntelligentPlant.DataCore.Client.Model;
using IntelligentPlant.DataCore.Client.Model.Scripting;
using IntelligentPlant.DataCore.Client.Queries;
using IntelligentPlant.Relativity;

namespace IntelligentPlant.DataCore.Client {

    /// <summary>
    /// Extensions for <see cref="DataCoreHttpClient"/>.
    /// </summary>
    public static class DataCoreHttpClientExtensions {

        #region [ Timestamp/Sample Interval Parsing ]

        /// <summary>
        /// Converts an absolute or relative time stamp into a UTC <see cref="DateTime"/>.
        /// </summary>
        /// <param name="absoluteOrRelativeTime">
        ///   The absolute or relative time stamp literal.
        /// </param>
        /// <param name="parameterName">
        ///   The name of the parameter in the calling method that is being parsed. This will be 
        ///   included in the <see cref="ArgumentException"/> thrown if <paramref name="absoluteOrRelativeTime"/> 
        ///   cannot be converted to a <see cref="DateTime"/>.
        /// </param>
        /// <returns>
        ///   The parsed UTC <see cref="DateTime"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///   <paramref name="absoluteOrRelativeTime"/> is not a valid absolute or relative timestamp.
        /// </exception>
        private static DateTime ParseTimestamp(string absoluteOrRelativeTime, string parameterName) { 
            if (!RelativityParser.Current.TryConvertToUtcDateTime(absoluteOrRelativeTime, null, out var result)) {
                throw new ArgumentException(Resources.Error_InvalidTimeStamp, parameterName);
            }

            return result;
        }


        /// <summary>
        /// Parses a long-hand or short-hand time span literal into a 
        /// <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="sampleInterval">
        ///   The long-hand or short-hand time span literal.
        /// </param>
        /// <param name="parameterName">
        ///   The name of the parameter in the calling method that is being parsed. This will be 
        ///   included in the <see cref="ArgumentException"/> thrown if <paramref name="sampleInterval"/> 
        ///   cannot be converted to a <see cref="TimeSpan"/>.
        /// </param>
        /// <returns>
        ///   The parsed <see cref="TimeSpan"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///   <paramref name="sampleInterval"/> is not a valid sample interval.
        /// </exception>
        private static TimeSpan ParseSampleInterval(string sampleInterval, string parameterName) {
            if (!RelativityParser.Current.TryConvertToTimeSpan(sampleInterval, out var result)) {
                throw new ArgumentException(Resources.Error_InvalidSampleInterval, parameterName);
            }

            return result;
        }

        #endregion

        #region [ General HTTP Requests ]

        /// <summary>
        /// Sends an HTTP request using the underlying <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="client">
        ///   The <see cref="DataCoreHttpClient"/>.
        /// </param>
        /// <param name="request">
        ///   The request to send.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A task that will return the HTTP response message.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="request"/> is <see langword="null"/>.
        /// </exception>
        public static async Task<HttpResponseMessage> SendAsync(this DataCoreHttpClient client, HttpRequestMessage request, CancellationToken cancellationToken = default) {
            return await client.HttpClient.SendAsync(request ?? throw new ArgumentNullException(nameof(request)), cancellationToken).ConfigureAwait(false);
        }


        /// <summary>
        /// Sends an HTTP GET request using the underlying <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="client">
        ///   The <see cref="DataCoreHttpClient"/>.
        /// </param>
        /// <param name="requestUri">
        ///   The request URL.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A task that will return the HTTP response message.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="requestUri"/> is <see langword="null"/>.
        /// </exception>
        public static async Task<HttpResponseMessage> GetAsync(this DataCoreHttpClient client, string requestUri, CancellationToken cancellationToken = default) {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, requestUri ?? throw new ArgumentNullException(nameof(requestUri)));
            var httpResponse = await client.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            return httpResponse;
        }


        /// <summary>
        /// Sends an HTTP GET request using the underlying <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="client">
        ///   The <see cref="DataCoreHttpClient"/>.
        /// </param>
        /// <param name="requestUri">
        ///   The request URL.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A task that will return the HTTP response message.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="requestUri"/> is <see langword="null"/>.
        /// </exception>
        public static async Task<HttpResponseMessage> GetAsync(this DataCoreHttpClient client, Uri requestUri, CancellationToken cancellationToken = default) {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, requestUri ?? throw new ArgumentNullException(nameof(requestUri)));
            var httpResponse = await client.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            return httpResponse;
        }


        /// <summary>
        /// Sends an HTTP POST request with a JSON-encoded request body using the underlying 
        /// <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="client">
        ///   The <see cref="DataCoreHttpClient"/>.
        /// </param>
        /// <param name="requestUri">
        ///   The request URL.
        /// </param>
        /// <param name="request">
        ///   The request object to serialize to JSON.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A task that will return the HTTP response message.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="requestUri"/> is <see langword="null"/>.
        /// </exception>
        public static async Task<HttpResponseMessage> PostAsJsonAsync<TRequest>(this DataCoreHttpClient client, string requestUri, TRequest request, CancellationToken cancellationToken = default) {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, requestUri ?? throw new ArgumentNullException(nameof(requestUri))) { Content = client.CreateJsonContent(request) };
            var httpResponse = await client.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            return httpResponse;
        }


        /// <summary>
        /// Sends an HTTP POST request with a JSON-encoded request body using the underlying 
        /// <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="client">
        ///   The <see cref="DataCoreHttpClient"/>.
        /// </param>
        /// <param name="requestUri">
        ///   The request URL.
        /// </param>
        /// <param name="request">
        ///   The request object to serialize to JSON.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A task that will return the HTTP response message.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="requestUri"/> is <see langword="null"/>.
        /// </exception>
        public static async Task<HttpResponseMessage> PostAsJsonAsync<TRequest>(this DataCoreHttpClient client, Uri requestUri, TRequest request, CancellationToken cancellationToken = default) {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, requestUri ?? throw new ArgumentNullException(nameof(requestUri))) { Content = client.CreateJsonContent(request) };
            var httpResponse = await client.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            return httpResponse;
        }


        /// <summary>
        /// Sends an HTTP PUT request with a JSON-encoded request body using the underlying 
        /// <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="client">
        ///   The <see cref="DataCoreHttpClient"/>.
        /// </param>
        /// <param name="requestUri">
        ///   The request URL.
        /// </param>
        /// <param name="request">
        ///   The request object to serialize to JSON.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A task that will return the HTTP response message.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="requestUri"/> is <see langword="null"/>.
        /// </exception>
        public static async Task<HttpResponseMessage> PutAsJsonAsync<TRequest>(this DataCoreHttpClient client, string requestUri, TRequest request, CancellationToken cancellationToken = default) {
            var httpRequest = new HttpRequestMessage(HttpMethod.Put, requestUri ?? throw new ArgumentNullException(nameof(requestUri))) { Content = client.CreateJsonContent(request) };
            var httpResponse = await client.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            return httpResponse;
        }


        /// <summary>
        /// Sends an HTTP PUT request with a JSON-encoded request body using the underlying 
        /// <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="client">
        ///   The <see cref="DataCoreHttpClient"/>.
        /// </param>
        /// <param name="requestUri">
        ///   The request URL.
        /// </param>
        /// <param name="request">
        ///   The request object to serialize to JSON.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A task that will return the HTTP response message.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="requestUri"/> is <see langword="null"/>.
        /// </exception>
        public static async Task<HttpResponseMessage> PutAsJsonAsync<TRequest>(this DataCoreHttpClient client, Uri requestUri, TRequest request, CancellationToken cancellationToken = default) {
            var httpRequest = new HttpRequestMessage(HttpMethod.Put, requestUri ?? throw new ArgumentNullException(nameof(requestUri))) { Content = client.CreateJsonContent(request) };
            var httpResponse = await client.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            return httpResponse;
        }


        /// <summary>
        /// Sends an HTTP DELETE request using the underlying <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="client">
        ///   The <see cref="DataCoreHttpClient"/>.
        /// </param>
        /// <param name="requestUri">
        ///   The request URL.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A task that will return the HTTP response message.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="requestUri"/> is <see langword="null"/>.
        /// </exception>
        public static async Task<HttpResponseMessage> DeleteAsync<TRequest>(this DataCoreHttpClient client, string requestUri, CancellationToken cancellationToken = default) {
            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, requestUri ?? throw new ArgumentNullException(nameof(requestUri)));
            var httpResponse = await client.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            return httpResponse;
        }


        /// <summary>
        /// Sends an HTTP DELETE request using the underlying <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="client">
        ///   The <see cref="DataCoreHttpClient"/>.
        /// </param>
        /// <param name="requestUri">
        ///   The request URL.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A task that will return the HTTP response message.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="requestUri"/> is <see langword="null"/>.
        /// </exception>
        public static async Task<HttpResponseMessage> DeleteAsync<TRequest>(this DataCoreHttpClient client, Uri requestUri, CancellationToken cancellationToken = default) {
            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, requestUri ?? throw new ArgumentNullException(nameof(requestUri)));
            var httpResponse = await client.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            return httpResponse;
        }

        #endregion

        #region [ Tag Searches ]

        /// <summary>
        /// Finds tags on the specified data source.
        /// </summary>
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
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The search results.
        /// </returns>
        public static Task<IEnumerable<TagSearchResult>> FindTagsAsync(
            this DataSourcesClient client, 
            string dataSourceName, 
            string? nameFilter = "*", 
            string? descriptionFilter = null, 
            string? unitsFilter = null,
            int page = 1,
            int pageSize = 20,
            CancellationToken cancellationToken = default
        ) {

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
                cancellationToken
            );
        }


        /// <summary>
        /// Gets tags on the specified data source by name or ID.
        /// </summary>
        /// <param name="client">
        ///   The Data Core client.
        /// </param>
        /// <param name="dataSourceName">
        ///   The data source name.
        /// </param>
        /// <param name="namesOrIds">
        ///   The names or IDs of the tags to retrieve.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The search results.
        /// </returns>
        public static Task<IEnumerable<TagSearchResult>> GetTagsAsync(
            this DataSourcesClient client,
            string dataSourceName,
            IEnumerable<string> namesOrIds,
            CancellationToken cancellationToken = default
        ) {
            if (client == null) {
                throw new ArgumentNullException(nameof(client));
            }

            return client.GetTagsAsync(
                new GetTagsRequest() {
                    DataSourceName = dataSourceName,
                    TagNamesOrIds = namesOrIds?.ToArray()!
                },
                cancellationToken
            );
        }


        /// <summary>
        /// Gets a tag on the specified data source by name or ID.
        /// </summary>
        /// <param name="client">
        ///   The Data Core client.
        /// </param>
        /// <param name="dataSourceName">
        ///   The data source name.
        /// </param>
        /// <param name="nameOrId">
        ///   The name or ID of the tag to retrieve.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The search results.
        /// </returns>
        public static async Task<TagSearchResult> GetTagAsync(
            this DataSourcesClient client,
            string dataSourceName,
            string nameOrId,
            CancellationToken cancellationToken = default
        ) {
            if (client == null) {
                throw new ArgumentNullException(nameof(client));
            }
            if (nameOrId == null) {
                throw new ArgumentNullException(nameof(nameOrId));
            }

            var result = await client.GetTagsAsync(
                new GetTagsRequest() {
                    DataSourceName = dataSourceName,
                    TagNamesOrIds = new [] { nameOrId }
                },
                cancellationToken
            );

            return result.FirstOrDefault()!;
        }


        /// <summary>
        /// Finds script tags on the specified data source.
        /// </summary>
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
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The search results.
        /// </returns>
        public static Task<IEnumerable<ScriptTagDefinition>> FindScriptTagsAsync(
            this DataSourcesClient client,
            string dataSourceName,
            string? nameFilter = "*",
            string? descriptionFilter = null,
            string? unitsFilter = null,
            int page = 1,
            int pageSize = 20,
            CancellationToken cancellationToken = default
        ) {

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
                cancellationToken
            );
        }


        /// <summary>
        /// Gets script tags on the specified data source.
        /// </summary>
        /// <param name="client">
        ///   The Data Core client.
        /// </param>
        /// <param name="dataSourceName">
        ///   The data source name.
        /// </param>
        /// <param name="namesOrIds">
        ///   The names or IDs of the script tags to retrieve.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The search results.
        /// </returns>
        public static Task<IEnumerable<ScriptTagDefinition>> GetScriptTagsAsync(
            this DataSourcesClient client,
            string dataSourceName,
            IEnumerable<string> namesOrIds,
            CancellationToken cancellationToken = default
        ) {
            if (client == null) {
                throw new ArgumentNullException(nameof(client));
            }

            return client.GetScriptTagsAsync(
                new GetTagsRequest() { 
                    DataSourceName = dataSourceName,
                    TagNamesOrIds = namesOrIds?.ToArray()!
                },
                cancellationToken
            );
        }


        /// <summary>
        /// Gets a single script tag on the specified data source.
        /// </summary>
        /// <param name="client">
        ///   The Data Core client.
        /// </param>
        /// <param name="dataSourceName">
        ///   The data source name.
        /// </param>
        /// <param name="nameOrId">
        ///   The name or ID of the script tag to retrieve.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The search results.
        /// </returns>
        public static Task<ScriptTagDefinition> GetScriptTagAsync(
            this DataSourcesClient client,
            string dataSourceName,
            string nameOrId,
            CancellationToken cancellationToken = default
        ) {

            if (client == null) {
                throw new ArgumentNullException(nameof(client));
            }

            return client.GetScriptTagAsync(
                new GetTagRequest() {
                    DataSourceName = dataSourceName,
                    TagNameOrId = nameOrId
                },
                cancellationToken
            );
        }

        #endregion

        #region [ Snapshot Tag Value Queries ]

        /// <summary>
        /// Gets snapshot (current) tag values.
        /// </summary>
        /// <param name="client">
        ///   The Data Core client.
        /// </param>
        /// <param name="tagMap">
        ///   A dictionary of tags to query, mapping from data source name to tag names.
        /// </param>
        /// <param name="properties">
        ///   Bespoke query properties.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The snapshot tag values, indexed by data source name and then tag name.
        /// </returns>
        public static Task<IDictionary<string, SnapshotTagValueDictionary>> ReadSnapshotTagValuesAsync(
            this DataSourcesClient client,
            IDictionary<string, string[]> tagMap,
            IDictionary<string, string>? properties = null,
            CancellationToken cancellationToken = default
        ) {
            if (client == null) {
                throw new ArgumentNullException(nameof(client));
            }

            return client.ReadSnapshotTagValuesAsync(
                new ReadSnapshotTagValuesRequest() {
                    Tags = tagMap,
                    QueryProperties = properties!
                },
                cancellationToken
            );
        }


        /// <summary>
        /// Gets snapshot (current) tag values.
        /// </summary>
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
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The snapshot tag values, indexed by tag name.
        /// </returns>
        public static async Task<SnapshotTagValueDictionary?> ReadSnapshotTagValuesAsync(
            this DataSourcesClient client,
            string dataSourceName,
            IEnumerable<string> tagNames,
            IDictionary<string, string>? properties = null,
            CancellationToken cancellationToken = default
        ) {
            if (client == null) {
                throw new ArgumentNullException(nameof(client));
            }

            if (string.IsNullOrWhiteSpace(dataSourceName)) {
                throw new ArgumentException(Resources.Error_DataSourceNameIsRequired, nameof(dataSourceName));
            }

            var result = await client.ReadSnapshotTagValuesAsync(
                new ReadSnapshotTagValuesRequest() {
                    Tags = new Dictionary<string, string[]>() {
                        { dataSourceName, tagNames?.Distinct()?.ToArray()! }
                    },
                    QueryProperties = properties
                },
                cancellationToken
            ).ConfigureAwait(false);

            return result?.FirstOrDefault().Value;
        }

        #endregion

        #region [ RAW Tag Value Queries ]

        /// <summary>
        /// Gets raw historical tag values.
        /// </summary>
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
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The historical tag values, indexed by data source name and then tag name.
        /// </returns>
        public static Task<IDictionary<string, HistoricalTagValuesDictionary>> ReadRawTagValuesAsync(
            this DataSourcesClient client,
            IDictionary<string, string[]> tagMap,
            DateTime utcStartTime,
            DateTime utcEndTime,
            int pointCount,
            IDictionary<string, string>? properties = null,
            CancellationToken cancellationToken = default
        ) {

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
                cancellationToken
            );
        }


        /// <summary>
        /// Gets raw historical tag values.
        /// </summary>
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
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The historical tag values, indexed by data source name and then tag name.
        /// </returns>
        /// <remarks>
        ///   <paramref name="absoluteOrRelativeStartTime"/> and <paramref name="absoluteOrRelativeEndTime"/> 
        ///   are parsed using <see cref="RelativityParser.Current"/>.
        /// </remarks>
        public static Task<IDictionary<string, HistoricalTagValuesDictionary>> ReadRawTagValuesAsync(
            this DataSourcesClient client,
            IDictionary<string, string[]> tagMap,
            string absoluteOrRelativeStartTime,
            string absoluteOrRelativeEndTime,
            int pointCount,
            IDictionary<string, string>? properties = null,
            CancellationToken cancellationToken = default
        ) {
            return ReadRawTagValuesAsync(
                client,
                tagMap,
                ParseTimestamp(absoluteOrRelativeStartTime, nameof(absoluteOrRelativeStartTime)),
                ParseTimestamp(absoluteOrRelativeEndTime, nameof(absoluteOrRelativeEndTime)),
                pointCount,
                properties,
                cancellationToken
            );
        }


        /// <summary>
        /// Gets raw historical tag values.
        /// </summary>
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
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The historical tag values, indexed by data source name and then tag name.
        /// </returns>
        public static async Task<HistoricalTagValuesDictionary?> ReadRawTagValuesAsync(
            this DataSourcesClient client,
            string dataSourceName,
            IEnumerable<string> tagNames,
            DateTime utcStartTime,
            DateTime utcEndTime,
            int pointCount,
            IDictionary<string, string>? properties = null,
            CancellationToken cancellationToken = default
        ) {

            if (string.IsNullOrWhiteSpace(dataSourceName)) {
                throw new ArgumentException(Resources.Error_DataSourceNameIsRequired, nameof(dataSourceName));
            }

            var result = await ReadRawTagValuesAsync(
                client,
                new Dictionary<string, string[]>() {
                    { dataSourceName, tagNames?.Distinct()?.ToArray()! }
                },
                utcStartTime,
                utcEndTime,
                pointCount,
                properties,
                cancellationToken
            ).ConfigureAwait(false);

            return result?.FirstOrDefault().Value;
        }


        /// <summary>
        /// Gets raw historical tag values.
        /// </summary>
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
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The historical tag values, indexed by data source name and then tag name.
        /// </returns>
        /// <remarks>
        ///   <paramref name="absoluteOrRelativeStartTime"/> and <paramref name="absoluteOrRelativeEndTime"/> 
        ///   are parsed using <see cref="RelativityParser.Current"/>.
        /// </remarks>
        public static Task<HistoricalTagValuesDictionary?> ReadRawTagValuesAsync(
            this DataSourcesClient client,
            string dataSourceName,
            IEnumerable<string> tagNames,
            string absoluteOrRelativeStartTime,
            string absoluteOrRelativeEndTime,
            int pointCount,
            IDictionary<string, string>? properties = null,
            CancellationToken cancellationToken = default
        ) {
            return ReadRawTagValuesAsync(
                client, 
                dataSourceName, 
                tagNames,
                ParseTimestamp(absoluteOrRelativeStartTime, nameof(absoluteOrRelativeStartTime)),
                ParseTimestamp(absoluteOrRelativeEndTime, nameof(absoluteOrRelativeEndTime)), 
                pointCount, 
                properties, 
                cancellationToken
            );
        }

        #endregion

        #region [ PLOT Tag Value Queries ]

        /// <summary>
        /// Gets visualization-friendly (plot) historical tag values, suitable for displaying on a 
        /// chart.
        /// </summary>
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
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The historical tag values, indexed by data source name and then tag name.
        /// </returns>
        public static Task<IDictionary<string, HistoricalTagValuesDictionary>> ReadPlotTagValuesAsync(
            this DataSourcesClient client,
            IDictionary<string, string[]> tagMap,
            DateTime utcStartTime,
            DateTime utcEndTime,
            int intervals,
            IDictionary<string, string>? properties = null,
            CancellationToken cancellationToken = default
        ) {

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
                cancellationToken
            );
        }


        /// <summary>
        /// Gets visualization-friendly (plot) historical tag values, suitable for displaying on a 
        /// chart.
        /// </summary>
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
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The historical tag values, indexed by data source name and then tag name.
        /// </returns>
        /// <remarks>
        ///   <paramref name="absoluteOrRelativeStartTime"/> and <paramref name="absoluteOrRelativeEndTime"/> 
        ///   are parsed using <see cref="RelativityParser.Current"/>.
        /// </remarks>
        public static Task<IDictionary<string, HistoricalTagValuesDictionary>> ReadPlotTagValuesAsync(
            this DataSourcesClient client,
            IDictionary<string, string[]> tagMap,
            string absoluteOrRelativeStartTime,
            string absoluteOrRelativeEndTime,
            int intervals,
            IDictionary<string, string>? properties = null,
            CancellationToken cancellationToken = default
        ) {
            return ReadPlotTagValuesAsync(
                client,
                tagMap,
                ParseTimestamp(absoluteOrRelativeStartTime, nameof(absoluteOrRelativeStartTime)),
                ParseTimestamp(absoluteOrRelativeEndTime, nameof(absoluteOrRelativeEndTime)),
                intervals,
                properties,
                cancellationToken
            );
        }


        /// <summary>
        /// Gets visualization-friendly (plot) historical tag values, suitable for displaying on a 
        /// chart.
        /// </summary>
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
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The historical tag values, indexed by data source name and then tag name.
        /// </returns>
        public static async Task<HistoricalTagValuesDictionary?> ReadPlotTagValuesAsync(
            this DataSourcesClient client,
            string dataSourceName,
            IEnumerable<string> tagNames,
            DateTime utcStartTime,
            DateTime utcEndTime,
            int intervals,
            IDictionary<string, string>? properties = null,
            CancellationToken cancellationToken = default
        ) {

            if (string.IsNullOrWhiteSpace(dataSourceName)) {
                throw new ArgumentException(Resources.Error_DataSourceNameIsRequired, nameof(dataSourceName));
            }

            var result = await ReadPlotTagValuesAsync(
                client,
                new Dictionary<string, string[]>() {
                    [dataSourceName] = tagNames?.Distinct()?.ToArray()!
                },
                utcStartTime,
                utcEndTime,
                intervals,
                properties,
                cancellationToken
            ).ConfigureAwait(false);

            return result?.FirstOrDefault().Value;
        }


        /// <summary>
        /// Gets visualization-friendly (plot) historical tag values, suitable for displaying on a 
        /// chart.
        /// </summary>
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
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The historical tag values, indexed by data source name and then tag name.
        /// </returns>
        /// <remarks>
        ///   <paramref name="absoluteOrRelativeStartTime"/> and <paramref name="absoluteOrRelativeEndTime"/> 
        ///   are parsed using <see cref="RelativityParser.Current"/>.
        /// </remarks>
        public static Task<HistoricalTagValuesDictionary?> ReadPlotTagValuesAsync(
            this DataSourcesClient client,
            string dataSourceName,
            IEnumerable<string> tagNames,
            string absoluteOrRelativeStartTime,
            string absoluteOrRelativeEndTime,
            int intervals,
            IDictionary<string, string>? properties = null,
            CancellationToken cancellationToken = default
        ) {
            return ReadPlotTagValuesAsync(
                client,
                dataSourceName,
                tagNames,
                ParseTimestamp(absoluteOrRelativeStartTime, nameof(absoluteOrRelativeStartTime)),
                ParseTimestamp(absoluteOrRelativeEndTime, nameof(absoluteOrRelativeEndTime)),
                intervals,
                properties,
                cancellationToken
            );
        }

        #endregion

        #region [ INTERP/AVG/MIN/MAX Tag Value Queries ]

        /// <summary>
        /// Gets processed (aggregated) historical tag values.
        /// </summary>
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
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The historical tag values, indexed by data source name and then tag name.
        /// </returns>
        public static Task<IDictionary<string, HistoricalTagValuesDictionary>> ReadProcessedTagValuesAsync(
            this DataSourcesClient client,
            IDictionary<string, string[]> tagMap,
            DateTime utcStartTime,
            DateTime utcEndTime,
            string dataFunction,
            TimeSpan sampleInterval,
            IDictionary<string, string>? properties = null,
            CancellationToken cancellationToken = default
        ) {

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
                cancellationToken
            );
        }


        /// <summary>
        /// Gets processed (aggregated) historical tag values.
        /// </summary>
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
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The historical tag values, indexed by data source name and then tag name.
        /// </returns>
        /// <remarks>
        ///   <paramref name="sampleInterval"/> is parsed using <see cref="RelativityParser.Current"/>.
        /// </remarks>
        public static Task<IDictionary<string, HistoricalTagValuesDictionary>> ReadProcessedTagValuesAsync(
            this DataSourcesClient client,
            IDictionary<string, string[]> tagMap,
            DateTime utcStartTime,
            DateTime utcEndTime,
            string dataFunction,
            string sampleInterval,
            IDictionary<string, string>? properties = null,
            CancellationToken cancellationToken = default
        ) {
            return ReadProcessedTagValuesAsync(
                client,
                tagMap,
                utcStartTime,
                utcEndTime,
                dataFunction,
                ParseSampleInterval(sampleInterval, nameof(sampleInterval)),
                properties,
                cancellationToken
            );
        }


        /// <summary>
        /// Gets processed (aggregated) historical tag values.
        /// </summary>
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
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The historical tag values, indexed by data source name and then tag name.
        /// </returns>
        /// <remarks>
        ///   <paramref name="absoluteOrRelativeStartTime"/> and <paramref name="absoluteOrRelativeEndTime"/> 
        ///   are parsed using <see cref="RelativityParser.Current"/>.
        /// </remarks>
        public static Task<IDictionary<string, HistoricalTagValuesDictionary>> ReadProcessedTagValuesAsync(
            this DataSourcesClient client,
            IDictionary<string, string[]> tagMap,
            string absoluteOrRelativeStartTime,
            string absoluteOrRelativeEndTime,
            string dataFunction,
            TimeSpan sampleInterval,
            IDictionary<string, string>? properties = null,
            CancellationToken cancellationToken = default
        ) {
            return ReadProcessedTagValuesAsync(
                client,
                tagMap,
                ParseTimestamp(absoluteOrRelativeStartTime, nameof(absoluteOrRelativeStartTime)),
                ParseTimestamp(absoluteOrRelativeEndTime, nameof(absoluteOrRelativeEndTime)),
                dataFunction,
                sampleInterval,
                properties,
                cancellationToken
            );
        }


        /// <summary>
        /// Gets processed (aggregated) historical tag values.
        /// </summary>
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
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The historical tag values, indexed by data source name and then tag name.
        /// </returns>
        /// <remarks>
        ///   <paramref name="absoluteOrRelativeStartTime"/>, <paramref name="absoluteOrRelativeEndTime"/> 
        ///   and <paramref name="sampleInterval"/> are parsed using <see cref="RelativityParser.Current"/>.
        /// </remarks>
        public static Task<IDictionary<string, HistoricalTagValuesDictionary>> ReadProcessedTagValuesAsync(
            this DataSourcesClient client,
            IDictionary<string, string[]> tagMap,
            string absoluteOrRelativeStartTime,
            string absoluteOrRelativeEndTime,
            string dataFunction,
            string sampleInterval,
            IDictionary<string, string>? properties = null,
            CancellationToken cancellationToken = default
        ) {
            return ReadProcessedTagValuesAsync(
                client,
                tagMap,
                ParseTimestamp(absoluteOrRelativeStartTime, nameof(absoluteOrRelativeStartTime)),
                ParseTimestamp(absoluteOrRelativeEndTime, nameof(absoluteOrRelativeEndTime)),
                dataFunction,
                sampleInterval,
                properties,
                cancellationToken
            );
        }


        /// <summary>
        /// Gets processed (aggregated) historical tag values.
        /// </summary>
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
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The historical tag values, indexed by data source name and then tag name.
        /// </returns>
        public static async Task<HistoricalTagValuesDictionary?> ReadProcessedTagValuesAsync(
            this DataSourcesClient client,
            string dataSourceName,
            IEnumerable<string> tagNames,
            DateTime utcStartTime,
            DateTime utcEndTime,
            string dataFunction,
            TimeSpan sampleInterval,
            IDictionary<string, string>? properties = null,
            CancellationToken cancellationToken = default
        ) {

            if (string.IsNullOrWhiteSpace(dataSourceName)) {
                throw new ArgumentException(Resources.Error_DataSourceNameIsRequired, nameof(dataSourceName));
            }

            var result = await ReadProcessedTagValuesAsync(
                client,
                new Dictionary<string, string[]>() {
                    { dataSourceName, tagNames?.Distinct()?.ToArray()! }
                },
                utcStartTime,
                utcEndTime,
                dataFunction,
                sampleInterval,
                properties,
                cancellationToken
            ).ConfigureAwait(false);

            return result?.FirstOrDefault().Value;
        }


        /// <summary>
        /// Gets processed (aggregated) historical tag values.
        /// </summary>
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
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The historical tag values, indexed by data source name and then tag name.
        /// </returns>
        /// <remarks>
        ///   <paramref name="sampleInterval"/> is parsed using <see cref="RelativityParser.Current"/>.
        /// </remarks>
        public static Task<HistoricalTagValuesDictionary?> ReadProcessedTagValuesAsync(
            this DataSourcesClient client,
            string dataSourceName,
            IEnumerable<string> tagNames,
            DateTime utcStartTime,
            DateTime utcEndTime,
            string dataFunction,
            string sampleInterval,
            IDictionary<string, string>? properties = null,
            CancellationToken cancellationToken = default
        ) {
            return ReadProcessedTagValuesAsync(
                client,
                dataSourceName,
                tagNames,
                utcStartTime,
                utcEndTime,
                dataFunction,
                ParseSampleInterval(sampleInterval, nameof(sampleInterval)),
                properties,
                cancellationToken
            );
        }


        /// <summary>
        /// Gets processed (aggregated) historical tag values.
        /// </summary>
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
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The historical tag values, indexed by data source name and then tag name.
        /// </returns>
        /// <remarks>
        ///   <paramref name="absoluteOrRelativeStartTime"/> and <paramref name="absoluteOrRelativeEndTime"/> 
        ///   are parsed using <see cref="RelativityParser.Current"/>.
        /// </remarks>
        public static Task<HistoricalTagValuesDictionary?> ReadProcessedTagValuesAsync(
            this DataSourcesClient client,
            string dataSourceName,
            IEnumerable<string> tagNames,
            string absoluteOrRelativeStartTime,
            string absoluteOrRelativeEndTime,
            string dataFunction,
            TimeSpan sampleInterval,
            IDictionary<string, string>? properties = null,
            CancellationToken cancellationToken = default
        ) {
            return ReadProcessedTagValuesAsync(
                client,
                dataSourceName,
                tagNames,
                ParseTimestamp(absoluteOrRelativeStartTime, nameof(absoluteOrRelativeStartTime)),
                ParseTimestamp(absoluteOrRelativeEndTime, nameof(absoluteOrRelativeEndTime)),
                dataFunction,
                sampleInterval,
                properties,
                cancellationToken
            );
        }


        /// <summary>
        /// Gets processed (aggregated) historical tag values.
        /// </summary>
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
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The historical tag values, indexed by data source name and then tag name.
        /// </returns>
        /// <remarks>
        ///   <paramref name="absoluteOrRelativeStartTime"/>, <paramref name="absoluteOrRelativeEndTime"/> 
        ///   and <paramref name="sampleInterval"/> are parsed using <see cref="RelativityParser.Current"/>.
        /// </remarks>
        public static Task<HistoricalTagValuesDictionary?> ReadProcessedTagValuesAsync(
            this DataSourcesClient client,
            string dataSourceName,
            IEnumerable<string> tagNames,
            string absoluteOrRelativeStartTime,
            string absoluteOrRelativeEndTime,
            string dataFunction,
            string sampleInterval,
            IDictionary<string, string>? properties = null,
            CancellationToken cancellationToken = default
        ) {
            return ReadProcessedTagValuesAsync(
                client,
                dataSourceName,
                tagNames,
                absoluteOrRelativeStartTime,
                absoluteOrRelativeEndTime,
                dataFunction,
                ParseSampleInterval(sampleInterval, nameof(sampleInterval)),
                properties,
                cancellationToken
            );
        }

        #endregion

        #region [ Tag Values At Times Queries ]

        /// <summary>
        /// Gets historical tag values at specific sample times.
        /// </summary>
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
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The historical tag values, indexed by data source name and then tag name.
        /// </returns>
        public static Task<IDictionary<string, HistoricalTagValuesDictionary>> ReadTagValuesAtTimesAsync(
            this DataSourcesClient client, 
            IDictionary<string, string[]> tagMap, 
            IEnumerable<DateTime> utcSampleTimes, 
            IDictionary<string, string>? properties = null,
            CancellationToken cancellationToken = default
        ) {

            if (client == null) {
                throw new ArgumentNullException(nameof(client));
            }

            return client.ReadTagValuesAtTimesAsync(
                new ReadTagValuesAtTimesRequest() {
                    Tags = tagMap,
                    UtcSampleTimes = utcSampleTimes?.Distinct()?.ToArray()!,
                    QueryProperties = properties
                },
                cancellationToken
            );
        }


        /// <summary>
        /// Gets historical tag values at specific sample times.
        /// </summary>
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
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The historical tag values, indexed by data source name and then tag name.
        /// </returns>
        /// <remarks>
        ///   The items in <paramref name="absoluteOrRelativeSampleTimes"/> are parsed using 
        ///   <see cref="RelativityParser.Current"/>.
        /// </remarks>
        public static Task<IDictionary<string, HistoricalTagValuesDictionary>> ReadTagValuesAtTimesAsync(
            this DataSourcesClient client,
            IDictionary<string, string[]> tagMap,
            IEnumerable<string> absoluteOrRelativeSampleTimes,
            IDictionary<string, string>? properties = null,
            CancellationToken cancellationToken = default
        ) {

            if (client == null) {
                throw new ArgumentNullException(nameof(client));
            }

            var sampleTimes = new List<DateTime>();
            if (absoluteOrRelativeSampleTimes != null) {
                foreach (var item in absoluteOrRelativeSampleTimes) {
                    sampleTimes.Add(ParseTimestamp(item, nameof(absoluteOrRelativeSampleTimes)));
                }
            }

            return client.ReadTagValuesAtTimesAsync(
                new ReadTagValuesAtTimesRequest() {
                    Tags = tagMap,
                    UtcSampleTimes = sampleTimes.Distinct().ToArray(),
                    QueryProperties = properties
                },
                cancellationToken
            );
        }


        /// <summary>
        /// Gets historical tag values at specific sample times.
        /// </summary>
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
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The historical tag values, indexed by data source name and then tag name.
        /// </returns>
        public static async Task<HistoricalTagValuesDictionary?> ReadTagValuesAtTimesAsync(
            this DataSourcesClient client,
            string dataSourceName,
            IEnumerable<string> tagNames,
            IEnumerable<DateTime> utcSampleTimes,
            IDictionary<string, string>? properties = null,
            CancellationToken cancellationToken = default
        ) {

            if (string.IsNullOrWhiteSpace(dataSourceName)) {
                throw new ArgumentException(Resources.Error_DataSourceNameIsRequired, nameof(dataSourceName));
            }

            var result = await ReadTagValuesAtTimesAsync(
                client,
                new Dictionary<string, string[]>() {
                    { dataSourceName, tagNames?.Distinct()?.ToArray()! }
                },
                utcSampleTimes,
                properties,
                cancellationToken
            ).ConfigureAwait(false);

            return result?.FirstOrDefault().Value;
        }


        /// <summary>
        /// Gets historical tag values at specific sample times.
        /// </summary>
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
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The historical tag values, indexed by data source name and then tag name.
        /// </returns>
        public static async Task<HistoricalTagValuesDictionary?> ReadTagValuesAtTimesAsync(
            this DataSourcesClient client,
            string dataSourceName,
            IEnumerable<string> tagNames,
            IEnumerable<string> absoluteOrRelativeSampleTimes,
            IDictionary<string, string>? properties = null,
            CancellationToken cancellationToken = default
        ) {

            if (string.IsNullOrWhiteSpace(dataSourceName)) {
                throw new ArgumentException(Resources.Error_DataSourceNameIsRequired, nameof(dataSourceName));
            }

            var sampleTimes = new List<DateTime>();
            if (absoluteOrRelativeSampleTimes != null) {
                foreach (var item in absoluteOrRelativeSampleTimes) {
                    sampleTimes.Add(ParseTimestamp(item, nameof(absoluteOrRelativeSampleTimes)));
                }
            }

            var result = await ReadTagValuesAtTimesAsync(
                client,
                new Dictionary<string, string[]>() {
                    { dataSourceName, tagNames?.Distinct()?.ToArray()! }
                },
                sampleTimes.Distinct().ToArray(),
                properties,
                cancellationToken
            ).ConfigureAwait(false);

            return result?.FirstOrDefault().Value;
        }

        #endregion

        #region [ Write Tag Value Operations ]

        /// <summary>
        /// Writes values to a data source's snapshot.
        /// </summary>
        /// <param name="client">
        ///   The client.
        /// </param>
        /// <param name="dataSourceName">
        ///   The data source to write to.
        /// </param>
        /// <param name="values">
        ///   The values to write.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A <see cref="TagValueUpdateResponse"/> describing the write results for each tag 
        ///   that was written to.
        /// </returns>
        public static Task<IEnumerable<TagValueUpdateResponse>> WriteSnapshotTagValuesAsync(
            this DataSourcesClient client,
            string dataSourceName,
            IEnumerable<TagValue> values,
            CancellationToken cancellationToken = default
        ) {
            if (client == null) {
                throw new ArgumentNullException(nameof(client));
            }

            if (string.IsNullOrWhiteSpace(dataSourceName)) {
                throw new ArgumentException(Resources.Error_DataSourceNameIsRequired, nameof(dataSourceName));
            }

            return client.WriteSnapshotTagValuesAsync(new WriteTagValuesRequest() { 
                DataSourceName = dataSourceName,
                Values = values?.Where(x => x != null).ToArray()!
            }, cancellationToken);
        }


        /// <summary>
        /// Writes numeric values to a single tag in a data source's snapshot.
        /// </summary>
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
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A <see cref="TagValueUpdateResponse"/> describing the write results for the tag.
        /// </returns>
        public static async Task<TagValueUpdateResponse?> WriteSnapshotTagValuesAsync(
            this DataSourcesClient client,
            string dataSourceName,
            string tagName,
            IDictionary<DateTime, double> values,
            TagValueStatus status = TagValueStatus.Good,
            CancellationToken cancellationToken = default
        ) {
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
                values?.OrderBy(x => x.Key).Select(x => new TagValueBuilder(tagName)
                    .WithTimestamp(x.Key)
                    .WithNumericValue(x.Value)
                    .WithStatus(status)
                    .Build())!,
                cancellationToken
            ).ConfigureAwait(false);

            return result?.FirstOrDefault();
        }


        /// <summary>
        /// Writes text values to a single tag in a data source's snapshot.
        /// </summary>
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
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A <see cref="TagValueUpdateResponse"/> describing the write results for the tag.
        /// </returns>
        public static async Task<TagValueUpdateResponse?> WriteSnapshotTagValuesAsync(
            this DataSourcesClient client,
            string dataSourceName,
            string tagName,
            IDictionary<DateTime, string> values,
            TagValueStatus status = TagValueStatus.Good,
            CancellationToken cancellationToken = default
        ) {
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
                values?.OrderBy(x => x.Key).Select(x => new TagValueBuilder(tagName)
                    .WithTimestamp(x.Key)
                    .WithDisplayValue(x.Value)
                    .WithStatus(status)
                    .Build())!,
                cancellationToken
            ).ConfigureAwait(false);

            return result?.FirstOrDefault();
        }


        /// <summary>
        /// Writes a single numeric value to a single tag in a data source's snapshot.
        /// </summary>
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
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A <see cref="TagValueUpdateResponse"/> describing the write results for the tag.
        /// </returns>
        public static async Task<TagValueUpdateResponse?> WriteSnapshotTagValueAsync(
            this DataSourcesClient client,
            string dataSourceName,
            string tagName,
            DateTime utcSampleTime,
            double value,
            TagValueStatus status = TagValueStatus.Good,
            CancellationToken cancellationToken = default
        ) {
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
                    new TagValueBuilder(tagName)
                        .WithTimestamp(utcSampleTime)
                        .WithNumericValue(value)
                        .WithStatus(status)
                        .Build()
                },
                cancellationToken
            ).ConfigureAwait(false);

            return result?.FirstOrDefault();
        }


        /// <summary>
        /// Writes a single text value to a single tag in a data source's snapshot.
        /// </summary>
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
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A <see cref="TagValueUpdateResponse"/> describing the write results for the tag.
        /// </returns>
        public static async Task<TagValueUpdateResponse?> WriteSnapshotTagValueAsync(
            this DataSourcesClient client,
            string dataSourceName,
            string tagName,
            DateTime utcSampleTime,
            string value,
            TagValueStatus status = TagValueStatus.Good,
            CancellationToken cancellationToken = default
        ) {
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
                    new TagValueBuilder(tagName)
                        .WithTimestamp(utcSampleTime)
                        .WithDisplayValue(value)
                        .WithStatus(status)
                        .Build()
                },
                cancellationToken
            ).ConfigureAwait(false);

            return result?.FirstOrDefault();
        }


        /// <summary>
        /// Writes values to a data source's history archive.
        /// </summary>
        /// <param name="client">
        ///   The client.
        /// </param>
        /// <param name="dataSourceName">
        ///   The data source to write to.
        /// </param>
        /// <param name="values">
        ///   The values to write.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A <see cref="TagValueUpdateResponse"/> describing the write results for each tag 
        ///   that was written to.
        /// </returns>
        public static Task<IEnumerable<TagValueUpdateResponse>> WriteHistoricalTagValuesAsync(
            this DataSourcesClient client,
            string dataSourceName,
            IEnumerable<TagValue> values,
            CancellationToken cancellationToken = default
        ) {
            if (client == null) {
                throw new ArgumentNullException(nameof(client));
            }

            if (string.IsNullOrWhiteSpace(dataSourceName)) {
                throw new ArgumentException(Resources.Error_DataSourceNameIsRequired, nameof(dataSourceName));
            }

            return client.WriteHistoricalTagValuesAsync(new WriteTagValuesRequest() {
                DataSourceName = dataSourceName,
                Values = values?.Where(x => x != null).ToArray()!
            }, cancellationToken);
        }


        /// <summary>
        /// Writes numeric values to a single tag in a data source's history archive.
        /// </summary>
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
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A <see cref="TagValueUpdateResponse"/> describing the write results for each tag 
        ///   that was written to.
        /// </returns>
        public static async Task<TagValueUpdateResponse?> WriteHistoricalTagValuesAsync(
            this DataSourcesClient client,
            string dataSourceName,
            string tagName,
            IDictionary<DateTime, double> values,
            TagValueStatus status = TagValueStatus.Good,
            CancellationToken cancellationToken = default
        ) {
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
                values?.OrderBy(x => x.Key).Select(x => new TagValueBuilder(tagName)
                    .WithTimestamp(x.Key)
                    .WithNumericValue(x.Value)
                    .WithStatus(status)
                    .Build())!,
                cancellationToken
            ).ConfigureAwait(false);

            return result?.FirstOrDefault();
        }


        /// <summary>
        /// Writes text values to a single tag in a data source's history archive.
        /// </summary>
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
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A <see cref="TagValueUpdateResponse"/> describing the write results for each tag 
        ///   that was written to.
        /// </returns>
        public static async Task<TagValueUpdateResponse?> WriteHistoricalTagValuesAsync(
            this DataSourcesClient client,
            string dataSourceName,
            string tagName,
            IDictionary<DateTime, string> values,
            TagValueStatus status = TagValueStatus.Good,
            CancellationToken cancellationToken = default
        ) {
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
                values?.OrderBy(x => x.Key).Select(x => new TagValueBuilder(tagName)
                    .WithTimestamp(x.Key)
                    .WithDisplayValue(x.Value)
                    .WithStatus(status)
                    .Build())!,
                cancellationToken
            ).ConfigureAwait(false);

            return result?.FirstOrDefault();
        }

        #endregion

        #region [ Annotation Queries ]

        /// <summary>
        /// Reads annotations from a data source.
        /// </summary>
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
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A collection of <see cref="AnnotationCollection"/> describing the annotations for 
        ///   each tag in the query.
        /// </returns>
        public static async Task<IEnumerable<AnnotationCollection>> ReadAnnotationsAsync(
            this DataSourcesClient client,
            string dataSourceName,
            IEnumerable<string> tagNames,
            DateTime utcStartTime,
            DateTime utcEndTime,
            CancellationToken cancellationToken = default
        ) {
            if (client == null) {
                throw new ArgumentNullException(nameof(client));
            }

            if (string.IsNullOrWhiteSpace(dataSourceName)) {
                throw new ArgumentException(Resources.Error_DataSourceNameIsRequired, nameof(dataSourceName));
            }

            var result = await client.ReadAnnotationsAsync(new ReadAnnotationsRequest() { 
                DataSourceName = dataSourceName,
                TagNames = tagNames?.ToArray()!,
                StartTime = utcStartTime,
                EndTime = utcEndTime
            }, cancellationToken).ConfigureAwait(false);

            return result;
        }


        /// <summary>
        /// Reads annotations from a data source.
        /// </summary>
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
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A collection of <see cref="AnnotationCollection"/> describing the annotations for 
        ///   each tag in the query.
        /// </returns>
        /// <remarks>
        ///   <paramref name="absoluteOrRelativeStartTime"/> and <paramref name="absoluteOrRelativeEndTime"/> 
        ///   are parsed using <see cref="RelativityParser.Current"/>.
        /// </remarks>
        public static Task<IEnumerable<AnnotationCollection>> ReadAnnotationsAsync(
            this DataSourcesClient client,
            string dataSourceName,
            IEnumerable<string> tagNames,
            string absoluteOrRelativeStartTime,
            string absoluteOrRelativeEndTime,
            CancellationToken cancellationToken = default
        ) {
            if (client == null) {
                throw new ArgumentNullException(nameof(client));
            }

            if (string.IsNullOrWhiteSpace(dataSourceName)) {
                throw new ArgumentException(Resources.Error_DataSourceNameIsRequired, nameof(dataSourceName));
            }

            return ReadAnnotationsAsync(
                client,
                dataSourceName,
                tagNames,
                ParseTimestamp(absoluteOrRelativeStartTime, nameof(absoluteOrRelativeStartTime)),
                ParseTimestamp(absoluteOrRelativeEndTime, nameof(absoluteOrRelativeEndTime)),
                cancellationToken
            );
        }

        #endregion

    }
}
