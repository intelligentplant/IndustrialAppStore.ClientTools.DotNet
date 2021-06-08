using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IntelligentPlant.DataCore.Client.Clients {

    /// <summary>
    /// Provides information and diagnostic queries for Data Core.
    /// </summary>
    /// <typeparam name="TContext">
    ///   The context type that is passed to API calls to allow authentication headers to be added 
    ///   to outgoing requests.
    /// </typeparam>
    /// <typeparam name="TOptions">
    ///   The HTTP client options type.
    /// </typeparam>
    public class InfoClient<TContext, TOptions> : ClientBase<TOptions> where TOptions : DataCoreHttpClientOptions {

        /// <summary>
        /// Creates a new <see cref="InfoClient{TContext, TOptions}"/> object.
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
        public InfoClient(HttpClient httpClient, TOptions options) : base(httpClient, options) { }


        /// <summary>
        /// Gets the version of the remote Data Core endpoint.
        /// </summary>
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
        ///   The version of Data Core running at the remote endpoint.
        /// </returns>
        public async Task<string> GetDataCoreVersionAsync(
            TContext context = default, 
            CancellationToken cancellationToken = default
        ) {
            var url = GetAbsoluteUrl("api/info");

            using (var httpRequest = CreateHttpRequestMessage(HttpMethod.Get, url, context))
            using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                return await ReadFromJsonAsync<string>(httpResponse, cancellationToken).ConfigureAwait(false);
            }
        }

    }
}
