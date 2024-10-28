using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IntelligentPlant.DataCore.Client.Clients {

    /// <summary>
    /// Provides information and diagnostic queries for Data Core.
    /// </summary>
    public class InfoClient : ClientBase {

        /// <summary>
        /// Creates a new <see cref="InfoClient"/> object.
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
        internal InfoClient(HttpClient httpClient, DataCoreHttpClientOptions options) : base(httpClient, options) { }


        /// <summary>
        /// Gets the version of the remote Data Core endpoint.
        /// </summary>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The version of Data Core running at the remote endpoint.
        /// </returns>
        public async Task<string> GetDataCoreVersionAsync(CancellationToken cancellationToken = default) {
            var url = GetAbsoluteUrl("api/info");

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);

            try {
                using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                    await httpResponse.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
            }
            finally {
                httpRequest.Dispose();
            }
        }

    }
}
