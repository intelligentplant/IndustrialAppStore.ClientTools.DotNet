using System.ComponentModel.DataAnnotations;

using IntelligentPlant.DataCore.Client.Model;
using IntelligentPlant.DataCore.Client.Model.AssetModel;
using IntelligentPlant.DataCore.Client.Queries;

namespace IntelligentPlant.DataCore.Client.Clients {

    /// <summary>
    /// Client for querying data source asset models.
    /// </summary>
    public class AssetModelClient : ClientBase {

        /// <summary>
        /// Creates a new <see cref="AssetModelClient"/> object.
        /// </summary>
        /// <param name="httpClient">
        ///   The HTTP client to use.
        /// </param>
        /// <param name="options">
        ///   The HTTP client options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="httpClient"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   <paramref name="options"/> is <see langword="null"/>.
        /// </exception>
        internal AssetModelClient(HttpClient httpClient, DataCoreHttpClientOptions options) : base(httpClient, options) { }


        /// <summary>
        /// Gets the data sources that support asset model browsing.
        /// </summary>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A task that will return the asset model data sources.
        /// </returns>
        public async Task<IEnumerable<DataSourceInfo>> GetAssetModelDataSources(
            CancellationToken cancellationToken = default
        ) {
            var url = GetAbsoluteUrl("api/assetmodel/datasources");

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            try {
                using (var response = await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false)) {
                    await response.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await ReadFromJsonAsync<IEnumerable<DataSourceInfo>>(response, cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                request.Dispose();
            }
        }


        /// <summary>
        /// Finds asset model element templates.
        /// </summary>
        /// <param name="request">
        ///   The request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A task that will return the asset model element templates.
        /// </returns>
        public async Task<IEnumerable<AssetModelElementTemplate>> FindAssetModelElementTemplatesAsync(
            FindAssetModelElementTemplatesRequest request, 
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request), true);

            var url = GetAbsoluteUrl($"api/assetmodel/{Uri.EscapeDataString(request.DataSourceName)}/templates?name={Uri.EscapeDataString(request.NameFilter ?? string.Empty)}");

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);

            try {
                using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                    await httpResponse.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await ReadFromJsonAsync<IEnumerable<AssetModelElementTemplate>>(httpResponse, cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                httpRequest.Dispose();
            }
        }


        /// <summary>
        /// Gets a single asset model element template.
        /// </summary>
        /// <param name="request">
        ///   The request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A task that will return the asset model element templates.
        /// </returns>
        public async Task<IEnumerable<AssetModelElementTemplate>> GetAssetModelElementTemplateAsync(
            GetAssetModelElementTemplateRequest request,
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request), true);

            var url = GetAbsoluteUrl($"api/assetmodel/{Uri.EscapeDataString(request.DataSourceName)}/templates/{Uri.EscapeDataString(request.Id)}");

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);

            try {
                using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                    await httpResponse.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await ReadFromJsonAsync<IEnumerable<AssetModelElementTemplate>>(httpResponse, cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                httpRequest.Dispose();
            }
        }


        /// <summary>
        /// Finds asset model element template properties.
        /// </summary>
        /// <param name="request">
        ///   The request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A task that will return the asset model element template properties.
        /// </returns>
        public async Task<IEnumerable<AssetModelPropertyTemplate>> FindAssetModelPropertyTemplatesAsync(
            FindAssetModelPropertyTemplatesRequest request,
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request), true);

            var url = GetAbsoluteUrl($"api/assetmodel/{Uri.EscapeDataString(request.DataSourceName)}/templates/{Uri.EscapeDataString(request.Id)}/properties?name={Uri.EscapeDataString(request.NameFilter ?? string.Empty)}");

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);

            try {
                using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                    await httpResponse.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await ReadFromJsonAsync<IEnumerable<AssetModelPropertyTemplate>>(httpResponse, cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                httpRequest.Dispose();
            }
        }


        /// <summary>
        /// Finds asset model elements.
        /// </summary>
        /// <param name="request">
        ///   The request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A task that will return the asset model elements.
        /// </returns>
        public async Task<IEnumerable<AssetModelElement>> FindAssetModelElementsAsync(
            FindAssetModelElementsRequest request,
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request), true);

            var url = GetAbsoluteUrl(
                string.IsNullOrWhiteSpace(request.ParentId)
                    ? $"api/assetmodel/{Uri.EscapeDataString(request.DataSourceName)}/elements?name={Uri.EscapeDataString(request.NameFilter ?? string.Empty)}&propertyName={Uri.EscapeDataString(request.PropertyNameFilter ?? string.Empty)}&properties={request.LoadProperties}&children={request.LoadChildren}"
                    : $"api/assetmodel/{Uri.EscapeDataString(request.DataSourceName)}/elements/{Uri.EscapeDataString(request.ParentId)}/children?name={Uri.EscapeDataString(request.NameFilter ?? string.Empty)}&propertyName={Uri.EscapeDataString(request.PropertyNameFilter ?? string.Empty)}&properties={request.LoadProperties}&children={request.LoadChildren}"
            );

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);

            try {
                using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                    await httpResponse.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await ReadFromJsonAsync<IEnumerable<AssetModelElement>>(httpResponse, cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                httpRequest.Dispose();
            }
        }


        /// <summary>
        /// Gets a single asset model element.
        /// </summary>
        /// <param name="request">
        ///   The request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A task that will return the asset model element.
        /// </returns>
        public async Task<AssetModelElement> GetAssetModelElementAsync(
            GetAssetModelElementRequest request,
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request), true);

            var url = GetAbsoluteUrl($"api/assetmodel/{Uri.EscapeDataString(request.DataSourceName)}/elements/{Uri.EscapeDataString(request.Id)}?properties={request.LoadProperties}&children={request.LoadChildren}");
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);

            try {
                using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                    await httpResponse.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await ReadFromJsonAsync<AssetModelElement>(httpResponse, cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                httpRequest.Dispose();
            }
        }

    }
}
