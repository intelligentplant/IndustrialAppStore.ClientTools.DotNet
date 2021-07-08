using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

using IntelligentPlant.DataCore.Client.Queries;
using IntelligentPlant.DataCore.Client.Model;
using IntelligentPlant.DataCore.Client.Model.AssetModel;
using System.Text.Json;

namespace IntelligentPlant.DataCore.Client.Clients {

    /// <summary>
    /// Client for querying data source asset models.
    /// </summary>
    /// <typeparam name="TContext">
    ///   The context type that is passed to API calls to allow authentication headers to be added 
    ///   to outgoing requests.
    /// </typeparam>
    /// <typeparam name="TOptions">
    ///   The HTTP client options type.
    /// </typeparam>
    public class AssetModelClient<TContext, TOptions> : ClientBase<TOptions> where TOptions : DataCoreHttpClientOptions {

        /// <summary>
        /// Creates a new <see cref="AssetModelClient{TContext, TOptions}"/> object.
        /// </summary>
        /// <param name="httpClient">
        ///   The HTTP client to use.
        /// </param>
        /// <param name="options">
        ///   The HTTP client options.
        /// </param>
        /// <param name="jsonOptions">
        ///   JSON serializer options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="httpClient"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   <paramref name="options"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   <paramref name="jsonOptions"/> is <see langword="null"/>.
        /// </exception>
        public AssetModelClient(HttpClient httpClient, TOptions options, JsonSerializerOptions jsonOptions) : base(httpClient, options, jsonOptions) { }


        /// <summary>
        /// Gets the data sources that support asset model browsing.
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
        ///   A task that will return the asset model data sources.
        /// </returns>
        public async Task<IEnumerable<DataSourceInfo>> GetAssetModelDataSources(
            TContext context = default, 
            CancellationToken cancellationToken = default
        ) {
            var url = GetAbsoluteUrl("api/assetmodel/datasources");

            using (var httpRequest = CreateHttpRequestMessage(HttpMethod.Get, url, context))
            using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                return await ReadFromJsonAsync<IEnumerable<DataSourceInfo>>(httpResponse, cancellationToken).ConfigureAwait(false);
            }
        }


        /// <summary>
        /// Finds asset model element templates.
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
        ///   A task that will return the asset model element templates.
        /// </returns>
        public async Task<IEnumerable<AssetModelElementTemplate>> FindAssetModelElementTemplatesAsync(
            FindAssetModelElementTemplatesRequest request, 
            TContext context = default, 
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request), true);

            var url = GetAbsoluteUrl($"api/assetmodel/{Uri.EscapeDataString(request.DataSourceName)}/templates?name={Uri.EscapeDataString(request.NameFilter ?? string.Empty)}");

            using (var httpRequest = CreateHttpRequestMessage(HttpMethod.Get, url, context))
            using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                return await ReadFromJsonAsync<IEnumerable<AssetModelElementTemplate>>(httpResponse, cancellationToken).ConfigureAwait(false);
            }
        }


        /// <summary>
        /// Gets a single asset model element template.
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
        ///   A task that will return the asset model element templates.
        /// </returns>
        public async Task<IEnumerable<AssetModelElementTemplate>> GetAssetModelElementTemplateAsync(
            GetAssetModelElementTemplateRequest request, 
            TContext context = default, 
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request), true);

            var url = GetAbsoluteUrl($"api/assetmodel/{Uri.EscapeDataString(request.DataSourceName)}/templates/{Uri.EscapeDataString(request.Id)}");

            using (var httpRequest = CreateHttpRequestMessage(HttpMethod.Get, url, context))
            using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                return await ReadFromJsonAsync<IEnumerable<AssetModelElementTemplate>>(httpResponse, cancellationToken).ConfigureAwait(false);
            }
        }


        /// <summary>
        /// Finds asset model element template properties.
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
        ///   A task that will return the asset model element template properties.
        /// </returns>
        public async Task<IEnumerable<AssetModelPropertyTemplate>> FindAssetModelPropertyTemplatesAsync(
            FindAssetModelPropertyTemplatesRequest request,
            TContext context = default, 
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request), true);

            var url = GetAbsoluteUrl($"api/assetmodel/{Uri.EscapeDataString(request.DataSourceName)}/templates/{Uri.EscapeDataString(request.Id)}/properties?name={Uri.EscapeDataString(request.NameFilter ?? string.Empty)}");

            using (var httpRequest = CreateHttpRequestMessage(HttpMethod.Get, url, context))
            using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                return await ReadFromJsonAsync<IEnumerable<AssetModelPropertyTemplate>>(httpResponse, cancellationToken).ConfigureAwait(false);
            }
        }


        /// <summary>
        /// Finds asset model elements.
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
        ///   A task that will return the asset model elements.
        /// </returns>
        public async Task<IEnumerable<AssetModelElement>> FindAssetModelElementsAsync(
            FindAssetModelElementsRequest request,
            TContext context = default, 
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

            using (var httpRequest = CreateHttpRequestMessage(HttpMethod.Get, url, context))
            using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                return await ReadFromJsonAsync<IEnumerable<AssetModelElement>>(httpResponse, cancellationToken).ConfigureAwait(false);
            }
        }


        /// <summary>
        /// Gets a single asset model element.
        /// </summary>
        /// <param name="request">
        ///   The request.
        /// </param>
        /// <param name="context">
        ///   The contxt for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, this will be 
        ///   passed to the handler's callback when requesting the <c>Authorize</c> header value 
        ///   for the outgoing request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A task that will return the asset model element.
        /// </returns>
        public async Task<AssetModelElement> GetAssetModelElementAsync(
            GetAssetModelElementRequest request,
            TContext context = default, 
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request), true);

            var url = GetAbsoluteUrl($"api/assetmodel/{Uri.EscapeDataString(request.DataSourceName)}/elements/{Uri.EscapeDataString(request.Id)}?properties={request.LoadProperties}&children={request.LoadChildren}");

            using (var httpRequest = CreateHttpRequestMessage(HttpMethod.Get, url, context))
            using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                return await ReadFromJsonAsync<AssetModelElement>(httpResponse, cancellationToken).ConfigureAwait(false);
            }
        }

    }
}
