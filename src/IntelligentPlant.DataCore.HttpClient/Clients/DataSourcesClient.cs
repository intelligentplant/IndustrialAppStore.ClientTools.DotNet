using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using IntelligentPlant.DataCore.Client.Queries;
using IntelligentPlant.DataCore.Client.Model;
using IntelligentPlant.DataCore.Client.Model.Queries;
using IntelligentPlant.DataCore.Client.Model.Scripting;
using IntelligentPlant.DataCore.Client.Model.Scripting.Templates;

namespace IntelligentPlant.DataCore.Client.Clients {

    /// <summary>
    /// Client for performing Data Core data source queries.
    /// </summary>
    /// <typeparam name="TContext">
    ///   The context type that is passed to API calls to allow authentication headers to be added 
    ///   to outgoing requests.
    /// </typeparam>
    /// <typeparam name="TOptions">
    ///   The HTTP client options type.
    /// </typeparam>
    public class DataSourcesClient<TContext, TOptions> : ClientBase<TOptions> where TOptions : DataCoreHttpClientOptions {

        #region [ Constructor ]

        /// <summary>
        /// Creates a new <see cref="DataSourcesClient{TContext, TOptions}"/> object.
        /// </summary>
        /// <param name="httpClient">
        ///   The HTTP client to use.
        /// </param>
        /// <param name="options">
        ///   The HTTP client options to use.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="httpClient"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="options"/> is <see langword="null"/>.
        /// </exception>
        public DataSourcesClient(HttpClient httpClient, TOptions options) : base(httpClient, options) { }

        #endregion

        #region [ Discovery / Authorization ]

        /// <summary>
        /// Gets information about running data sources.
        /// </summary>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, this will be 
        ///   passed to the handler's callback when requesting the <c>Authorize</c> header value 
        ///   for the outgoing request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A task that will return information about the running data sources.
        /// </returns>
        public async Task<IEnumerable<DataSourceInfo>> GetDataSourcesAsync(
            TContext? context = default, 
            CancellationToken cancellationToken = default
        ) {
            var url = GetAbsoluteUrl("api/data/datasources");

            var request = new HttpRequestMessage(HttpMethod.Get, url).AddStateProperty(context);

            try {
                using (var response = await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false)) {
                    await response.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await response.Content.ReadAsAsync<IEnumerable<DataSourceInfo>>(cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                request.Dispose();
            }
        }


        /// <summary>
        /// Gets information about a running data source.
        /// </summary>
        /// <param name="dataSourceName">
        ///   The data source name.
        /// </param>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, this will be 
        ///   passed to the handler's callback when requesting the <c>Authorize</c> header value 
        ///   for the outgoing request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A task that will return information about the data source.
        /// </returns>
        public async Task<DataSourceInfo> GetDataSourceAsync(
            string dataSourceName,
            TContext? context = default, 
            CancellationToken cancellationToken = default
        ) {
            if (string.IsNullOrWhiteSpace(dataSourceName)) {
                throw new ArgumentException(Resources.Error_DataSourceNameIsRequired, nameof(dataSourceName));
            }

            var url = GetAbsoluteUrl($"api/data/datasources/{Uri.EscapeDataString(dataSourceName)}");

            var request = new HttpRequestMessage(HttpMethod.Get, url).AddStateProperty(context);

            try {
                using (var response = await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false)) {
                    await response.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await response.Content.ReadAsAsync<DataSourceInfo>(cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                request.Dispose();
            }
        }


        /// <summary>
        /// Tests if the user is authorized in any of the specified roles on a data source.
        /// </summary>
        /// <param name="request">
        ///   The request.
        /// </param>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, this will be 
        ///   passed to the handler's callback when requesting the <c>Authorize</c> header value 
        ///   for the outgoing request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A task that will return the authorization check results.
        /// </returns>
        public async Task<ComponentRoles> IsAuthorizedAsync(
            IsAuthorizedOnDataSourceRequest request,
            TContext? context = default, 
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request), true);
            var roleNames = request.RoleNames?.Where(x => !string.IsNullOrWhiteSpace(x))?.Distinct()?.ToArray();

            if (roleNames == null || !roleNames.Any()) {
                throw new ArgumentException(Resources.Error_OneOrMoreRoleNamesRequired, nameof(roleNames));
            }

            var url = GetAbsoluteUrl($"api/security/data-source/{Uri.EscapeDataString(request.DataSourceName)}/is-in-role");

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url) {
                Content = new ObjectContent<IEnumerable<string>>(roleNames, new JsonMediaTypeFormatter())
            }.AddStateProperty(context);

            try {
                using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                    await httpResponse.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await httpResponse.Content.ReadAsAsync<ComponentRoles>(cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                httpRequest.Dispose();
            }
        }

        #endregion

        #region [ Tag Searches ]

        /// <summary>
        /// Finds tags on the specified data source.
        /// </summary>
        /// <param name="request">
        ///   The search request.
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
        public async Task<IEnumerable<TagSearchResult>> FindTagsAsync(
            FindTagsRequest request,
            TContext? context = default, 
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request), true);

            var url = GetAbsoluteUrl($"api/data/tags/{Uri.EscapeDataString(request.DataSourceName)}");

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url) {
                Content = new ObjectContent(typeof(TagSearchFilter), request.Filter, new JsonMediaTypeFormatter())
            }.AddStateProperty(context);

            try {
                using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                    await httpResponse.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await httpResponse.Content.ReadAsAsync<IEnumerable<TagSearchResult>>(cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                httpRequest.Dispose();
            }
        }

        #endregion

        #region [ Read Snapshot Tag Values ]

        /// <summary>
        /// Gets snapshot (current) tag values.
        /// </summary>
        /// <param name="request">
        ///   The snapshot request.
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
        public async Task<IDictionary<string, SnapshotTagValueDictionary>> ReadSnapshotTagValuesAsync(
            ReadSnapshotTagValuesRequest request,
            TContext? context = default, 
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request), true);

            var url = GetAbsoluteUrl("api/data/v2/snapshot");

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url) {
                Content = new ObjectContent(request.GetType(), request, new JsonMediaTypeFormatter())
            }.AddStateProperty(context);

            try {
                using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                    await httpResponse.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await httpResponse.Content.ReadAsAsync<IDictionary<string, SnapshotTagValueDictionary>>(cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                httpRequest.Dispose();
            }
        }

        #endregion

        #region [ Read Historical Tag Values ]

        /// <summary>
        /// Gets raw historical tag values.
        /// </summary>
        /// <param name="request">
        ///   The raw data request.
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
        public async Task<IDictionary<string, HistoricalTagValuesDictionary>> ReadRawTagValuesAsync(
            ReadRawTagValuesRequest request,
            TContext? context = default, 
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request), true);

            var url = GetAbsoluteUrl("api/data/v2/raw");

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url) {
                Content = new ObjectContent(request.GetType(), request, new JsonMediaTypeFormatter())
            }.AddStateProperty(context);

            try {
                using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                    await httpResponse.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await httpResponse.Content.ReadAsAsync<IDictionary<string, HistoricalTagValuesDictionary>>(cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                httpRequest.Dispose();
            }
        }


        /// <summary>
        /// Gets visualization-friendly (plot) historical tag values, suitable for displaying on a 
        /// chart.
        /// </summary>
        /// <param name="request">
        ///   The plot data request.
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
        public async Task<IDictionary<string, HistoricalTagValuesDictionary>> ReadPlotTagValuesAsync(
            ReadPlotTagValuesRequest request,
            TContext? context = default, 
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request), true);

            var url = GetAbsoluteUrl("api/data/v2/plot");

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url) {
                Content = new ObjectContent(request.GetType(), request, new JsonMediaTypeFormatter())
            }.AddStateProperty(context);

            try {
                using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                    await httpResponse.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await httpResponse.Content.ReadAsAsync<IDictionary<string, HistoricalTagValuesDictionary>>(cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                httpRequest.Dispose();
            }
        }


        /// <summary>
        /// Gets processed (aggregated) historical tag values.
        /// </summary>
        /// <param name="request">
        ///   The processed data request.
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
        public async Task<IDictionary<string, HistoricalTagValuesDictionary>> ReadProcessedTagValuesAsync(
            ReadProcessedTagValuesRequest request,
            TContext? context = default, 
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request), true);

            var url = GetAbsoluteUrl("api/data/v2/processed");

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url) {
                Content = new ObjectContent(request.GetType(), request, new JsonMediaTypeFormatter())
            }.AddStateProperty(context);

            try {
                using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                    await httpResponse.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await httpResponse.Content.ReadAsAsync<IDictionary<string, HistoricalTagValuesDictionary>>(cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                httpRequest.Dispose();
            }
        }


        /// <summary>
        /// Gets historical tag values at specific sample times.
        /// </summary>
        /// <param name="request">
        ///   The values-at-times data request.
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
        public async Task<IDictionary<string, HistoricalTagValuesDictionary>> ReadTagValuesAtTimesAsync(
            ReadTagValuesAtTimesRequest request, 
            TContext? context = default, 
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request), true);

            var url = GetAbsoluteUrl("api/data/v2/values-at-times");

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url) {
                Content = new ObjectContent(request.GetType(), request, new JsonMediaTypeFormatter())
            }.AddStateProperty(context);

            try {
                using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                    await httpResponse.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await httpResponse.Content.ReadAsAsync<IDictionary<string, HistoricalTagValuesDictionary>>(cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                httpRequest.Dispose();
            }
        }

        #endregion

        #region [ Write Snapshot Tag Values ]

        /// <summary>
        /// Writes values to a data source's snapshot.
        /// </summary>
        /// <param name="request">
        ///   The write request.
        /// </param>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, this will be 
        ///   passed to the handler's callback when requesting the <c>Authorize</c> header value 
        ///   for the outgoing request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A <see cref="TagValueUpdateResponse"/> describing the write results for each tag 
        ///   that was written to.
        /// </returns>
        public async Task<IEnumerable<TagValueUpdateResponse>> WriteSnapshotTagValuesAsync(
            WriteTagValuesRequest request,
            TContext? context = default, 
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request), true);

            var url = GetAbsoluteUrl($"api/data/v2/snapshot/{Uri.EscapeDataString(request.DataSourceName)}");
            var httpRequest = new HttpRequestMessage(HttpMethod.Put, url) {
                Content = new ObjectContent<IEnumerable<TagValue>>(request.Values, new JsonMediaTypeFormatter())
            }.AddStateProperty(context);

            try {
                using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                    await httpResponse.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await httpResponse.Content.ReadAsAsync<IEnumerable<TagValueUpdateResponse>>(cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                httpRequest.Dispose();
            }
        }

        #endregion

        #region [ Write Historical Tag Values ]

        /// <summary>
        /// Writes values to a data source's historical archive.
        /// </summary>
        /// <param name="request">
        ///   The write request.
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
        public async Task<IEnumerable<TagValueUpdateResponse>> WriteHistoricalTagValuesAsync(
            WriteTagValuesRequest request,
            TContext? context = default, 
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request), true);

            var url = GetAbsoluteUrl($"api/data/v2/history/{Uri.EscapeDataString(request.DataSourceName)}");
            var httpRequest = new HttpRequestMessage(HttpMethod.Put, url) {
                Content = new ObjectContent<IEnumerable<TagValue>>(request.Values, new JsonMediaTypeFormatter())
            }.AddStateProperty(context);

            try {
                using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                    await httpResponse.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await httpResponse.Content.ReadAsAsync<IEnumerable<TagValueUpdateResponse>>(cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                httpRequest.Dispose();
            }
        }

        #endregion

        #region [ Read Tag Value Annotations ]

        /// <summary>
        /// Reads annotations from a data source.
        /// </summary>
        /// <param name="request">
        ///   The read request.
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
        public async Task<IEnumerable<AnnotationCollection>> ReadAnnotationsAsync(
            ReadAnnotationsRequest request,
            TContext? context = default, 
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request), true);

            var url = GetAbsoluteUrl("api/data/annotations");
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url) {
                Content = new ObjectContent<ReadAnnotationsRequest>(request, new JsonMediaTypeFormatter())
            }.AddStateProperty(context);

            try {
                using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                    await httpResponse.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await httpResponse.Content.ReadAsAsync<IEnumerable<AnnotationCollection>>(cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                httpRequest.Dispose();
            }
        }

        #endregion

        #region [ Write Tag Value Annotations ]

        /// <summary>
        /// Creates a tag value annotation.
        /// </summary>
        /// <param name="request">
        ///   The request.
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
        ///   A task that will return the created annotation.
        /// </returns>
        public async Task<Annotation> CreateAnnotationAsync(
            CreateAnnotationRequest request,
            TContext? context = default, 
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request), true);

            var url = GetAbsoluteUrl($"api/data/annotations/{Uri.EscapeDataString(request.DataSourceName)}");
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url) {
                Content = new ObjectContent<Annotation>(request.Annotation, new JsonMediaTypeFormatter())
            }.AddStateProperty(context);

            try {
                using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                    await httpResponse.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await httpResponse.Content.ReadAsAsync<Annotation>(cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                httpRequest.Dispose();
            }
        }


        /// <summary>
        /// Updates an existing tag value annotation.
        /// </summary>
        /// <param name="request">
        ///   The request.
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
        ///   A task that will complete when the update has finished.
        /// </returns>
        public async Task UpdateAnnotationAsync(
            UpdateAnnotationRequest request,
            TContext? context = default, 
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request), true);

            var url = GetAbsoluteUrl($"api/data/annotations/{Uri.EscapeDataString(request.DataSourceName)}");
            var httpRequest = new HttpRequestMessage(HttpMethod.Put, url) {
                Content = new ObjectContent<AnnotationUpdate>(request.Annotation, new JsonMediaTypeFormatter())
            }.AddStateProperty(context);

            try {
                using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                    await httpResponse.ThrowOnErrorResponse().ConfigureAwait(false);
                    await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
            }
            finally {
                httpRequest.Dispose();
            }
        }


        /// <summary>
        /// Deletes a tag value annotation.
        /// </summary>
        /// <param name="request">
        ///   The request.
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
        ///   A task that will return when the operation has completed.
        /// </returns>
        public async Task DeleteAnnotationAsync(
            DeleteAnnotationRequest request,
            TContext? context = default, 
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request), true);

            var url = GetAbsoluteUrl($"api/data/annotations/{Uri.EscapeDataString(request.DataSourceName)}?id={Uri.EscapeDataString(request.Annotation.Id)}&tagName={Uri.EscapeDataString(request.Annotation.TagName)}&utcAnnotationTime={Uri.EscapeDataString(request.Annotation.UtcAnnotationTime.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ"))}");
            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, url).AddStateProperty(context);

            try {
                using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                    await httpResponse.ThrowOnErrorResponse().ConfigureAwait(false);
                    await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
            }
            finally {
                httpRequest.Dispose();
            }
        }

        #endregion

        #region [ Script Tags ]

        /// <summary>
        /// Gets the script engines available for running script tags on the specified data source.
        /// </summary>
        /// <param name="request">
        ///   The request.
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
        ///   A task that will return the available script engines.
        /// </returns>
        public async Task<IEnumerable<ScriptEngine>> GetScriptEnginesAsync(
            GetDataSourceScriptEnginesRequest request,
            TContext? context = default, 
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request), true);

            var url = GetAbsoluteUrl($"api/configuration/scripting/tags/{Uri.EscapeDataString(request.DataSourceName)}/script-engines");

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, url).AddStateProperty(context);

            try {
                using (var response = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                    await response.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await response.Content.ReadAsAsync<IEnumerable<ScriptEngine>>(cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                httpRequest.Dispose();
            }
        }


        /// <summary>
        /// Finds available script tag templates on a data source.
        /// </summary>
        /// <param name="request">
        ///   The request.
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
        ///   A task that will return the available script tag templates.
        /// </returns>
        public async Task<IEnumerable<ScriptTemplate>> FindScriptTagTemplatesAsync(
            FindScriptTagTemplatesRequest request,
            TContext? context = default, 
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request), true);

            var url = GetAbsoluteUrl($"api/configuration/scripting/tags/{Uri.EscapeDataString(request.DataSourceName)}/script-engines/{Uri.EscapeDataString(request.ScriptEngineId)}/templates");

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, url).AddStateProperty(context);

            try {
                using (var response = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                    await response.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await response.Content.ReadAsAsync<IEnumerable<ScriptTemplate>>(cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                httpRequest.Dispose();
            }
        }


        /// <summary>
        /// Gets extended information about a script tag template on a data source.
        /// </summary>
        /// <param name="request">
        ///   The request.
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
        ///   A task that will return the script tag template.
        /// </returns>
        public async Task<ScriptTemplateWithParameterDefinitions> GetScriptTagTemplateAsync(
            GetScriptTagTemplateRequest request,
            TContext? context = default, 
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request), true);

            var url = GetAbsoluteUrl($"api/configuration/scripting/tags/{Uri.EscapeDataString(request.DataSourceName)}/script-engines/{Uri.EscapeDataString(request.ScriptEngineId)}/templates/{Uri.EscapeDataString(request.TemplateId)}");

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, url).AddStateProperty(context);

            try {
                using (var response = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                    await response.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await response.Content.ReadAsAsync<ScriptTemplateWithParameterDefinitions>(cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                httpRequest.Dispose();
            }
        }


        /// <summary>
        /// Performs a script tag search.
        /// </summary>
        /// <param name="request">
        ///   The request.
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
        ///   A task that will return the matching script tags.
        /// </returns>
        public async Task<IEnumerable<ScriptTagDefinition>> FindScriptTagsAsync(
            FindTagsRequest request,
            TContext? context = default, 
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request), true);

            var url = GetAbsoluteUrl($"api/configuration/scripting/tags/{Uri.EscapeDataString(request.DataSourceName)}/search");

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url) {
                Content = new ObjectContent(request.Filter.GetType(), request.Filter, new JsonMediaTypeFormatter())
            }.AddStateProperty(context);

            try {
                using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                    await httpResponse.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await httpResponse.Content.ReadAsAsync<IEnumerable<ScriptTagDefinition>>(cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                httpRequest.Dispose();
            }
        }


        /// <summary>
        /// Gets script tags by name or ID.
        /// </summary>
        /// <param name="request">
        ///   The request.
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
        ///   A task that will return the matching script tags.
        /// </returns>
        public async Task<IEnumerable<ScriptTagDefinition>> GetScriptTagsAsync(
            GetTagsRequest request,
            TContext? context = default, 
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request), true);

            var url = GetAbsoluteUrl($"api/configuration/scripting/tags/{Uri.EscapeDataString(request.DataSourceName)}");

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url) {
                Content = new ObjectContent(request.TagNamesOrIds.GetType(), request.TagNamesOrIds, new JsonMediaTypeFormatter())
            }.AddStateProperty(context);

            try {
                using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                    await httpResponse.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await httpResponse.Content.ReadAsAsync<IEnumerable<ScriptTagDefinition>>(cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                httpRequest.Dispose();
            }
        }


        /// <summary>
        /// Gets a single script tag definition.
        /// </summary>
        /// <param name="request">
        ///   The request.
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
        ///   A task that will return the matching script tag.
        /// </returns>
        public async Task<ScriptTagDefinition> GetScriptTagAsync(
            GetTagRequest request,
            TContext? context = default, 
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request), true);

            var url = GetAbsoluteUrl($"api/configuration/scripting/tags/{Uri.EscapeDataString(request.DataSourceName)}/{Uri.EscapeDataString(request.TagNameOrId)}");

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, url).AddStateProperty(context);

            try {
                using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                    await httpResponse.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await httpResponse.Content.ReadAsAsync<ScriptTagDefinition>(cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                httpRequest.Dispose();
            }
        }


        /// <summary>
        /// Creates a new ad hoc script tag.
        /// </summary>
        /// <param name="request">
        ///   The request.
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
        ///   A task that will return the new script tag.
        /// </returns>
        public async Task<ScriptTagDefinition> CreateScriptTagAsync(
            CreateScriptTagRequest request,
            TContext? context = default, 
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request), true);

            var url = GetAbsoluteUrl($"api/configuration/scripting/tags/{Uri.EscapeDataString(request.DataSourceName)}/create");

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url) {
                Content = new ObjectContent(request.Settings.GetType(), request.Settings, new JsonMediaTypeFormatter())
            }.AddStateProperty(context);

            try {
                using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                    await httpResponse.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await httpResponse.Content.ReadAsAsync<ScriptTagDefinition>(cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                httpRequest.Dispose();
            }
        }


        /// <summary>
        /// Creates a new script tag using a template.
        /// </summary>
        /// <param name="request">
        ///   The request.
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
        ///   A task that will return the new script tag.
        /// </returns>
        public async Task<ScriptTagDefinition> CreateScriptTagFromTemplateAsync(
            CreateTemplatedScriptTagRequest request,
            TContext? context = default, 
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request), true);

            var url = GetAbsoluteUrl($"api/configuration/scripting/tags/{Uri.EscapeDataString(request.DataSourceName)}/create-from-template");

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url) {
                Content = new ObjectContent(request.Settings.GetType(), request.Settings, new JsonMediaTypeFormatter())
            }.AddStateProperty(context);

            try {
                using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                    await httpResponse.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await httpResponse.Content.ReadAsAsync<ScriptTagDefinition>(cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                httpRequest.Dispose();
            }
        }


        /// <summary>
        /// Updates an existing ad hoc script tag.
        /// </summary>
        /// <param name="request">
        ///   The request.
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
        ///   A task that will return the updated script tag.
        /// </returns>
        public async Task<ScriptTagDefinition> UpdateScriptTagAsync(
            UpdateScriptTagRequest request,
            TContext? context = default, 
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request), true);

            var url = GetAbsoluteUrl($"api/configuration/scripting/tags/{Uri.EscapeDataString(request.DataSourceName)}/{Uri.EscapeDataString(request.ScriptTagId)}");

            var httpRequest = new HttpRequestMessage(HttpMethod.Put, url) {
                Content = new ObjectContent(request.Settings.GetType(), request.Settings, new JsonMediaTypeFormatter())
            }.AddStateProperty(context);

            try {
                using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                    await httpResponse.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await httpResponse.Content.ReadAsAsync<ScriptTagDefinition>(cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                httpRequest.Dispose();
            }
        }


        /// <summary>
        /// Updates an existing templated script tag.
        /// </summary>
        /// <param name="request">
        ///   The request.
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
        ///   A task that will return the updated script tag.
        /// </returns>
        public async Task<ScriptTagDefinition> UpdateScriptTagFromTemplateAsync(
            UpdateTemplatedScriptTagRequest request,
            TContext? context = default, 
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request), true);

            var url = GetAbsoluteUrl($"api/configuration/scripting/tags/{Uri.EscapeDataString(request.DataSourceName)}/update-from-template/{Uri.EscapeDataString(request.ScriptTagId)}");

            var httpRequest = new HttpRequestMessage(HttpMethod.Put, url) {
                Content = new ObjectContent(request.Settings.GetType(), request.Settings, new JsonMediaTypeFormatter())
            }.AddStateProperty(context);

            try {
                using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                    await httpResponse.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await httpResponse.Content.ReadAsAsync<ScriptTagDefinition>(cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                httpRequest.Dispose();
            }
        }


        /// <summary>
        /// Deletes a script tag.
        /// </summary>
        /// <param name="request">
        ///   The request.
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
        ///   A task that will return a flag indicating if the delete was successful.
        /// </returns>
        public async Task<bool> DeleteScriptTagAsync(
            DeleteScriptTagRequest request,
            TContext? context = default, 
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request), true);

            var url = GetAbsoluteUrl($"api/configuration/scripting/tags/{Uri.EscapeDataString(request.DataSourceName)}/{Uri.EscapeDataString(request.ScriptTagId)}");

            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, url).AddStateProperty(context);

            try {
                using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                    await httpResponse.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await httpResponse.Content.ReadAsAsync<bool>(cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                httpRequest.Dispose();
            }
        }

        #endregion

        #region [ Custom Functions ]

        /// <summary>
        /// Runs a custom function on a data source.
        /// </summary>
        /// <typeparam name="T">
        ///   The return type of the function.
        /// </typeparam>
        /// <param name="dataSourceName">
        ///   The data source to run the function on.
        /// </param>
        /// <param name="functionName">
        ///   The function to run.
        /// </param>
        /// <param name="parameters">
        ///   The function parameters.
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
        ///   A task that will return the custom function result.
        /// </returns>
        public async Task<T> RunCustomFunctionAsync<T>(
            string dataSourceName, 
            string functionName, 
            IDictionary<string, string>? parameters = null,
            TContext? context = default, 
            CancellationToken cancellationToken = default
        ) {
            if (string.IsNullOrWhiteSpace(dataSourceName)) {
                throw new ArgumentException(Resources.Error_DataSourceNameIsRequired, nameof(dataSourceName));
            }
            var request = new CustomFunctionRequest() { 
                ComponentName = dataSourceName,
                MethodName = functionName,
                Parameters = parameters == null
                    ? new Dictionary<string, string>()
                    : new Dictionary<string, string>(parameters)
            };

            return await CustomFunctionsClient<TContext, TOptions>.RunCustomFunctionAsync<T>(
                HttpClient,
                GetAbsoluteUrl("api/rpc")!,
                request, 
                context, 
                cancellationToken
            ).ConfigureAwait(false);
        }

        #endregion

    }
}
