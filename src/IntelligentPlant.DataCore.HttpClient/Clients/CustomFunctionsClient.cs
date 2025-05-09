using System.ComponentModel.DataAnnotations;

using IntelligentPlant.DataCore.Client.Model.Queries;

namespace IntelligentPlant.DataCore.Client.Clients {

    /// <summary>
    /// Client for performing custom function calls.
    /// </summary>
    public class CustomFunctionsClient : ClientBase {

        /// <summary>
        /// Creates a new <see cref="CustomFunctionsClient"/> object.
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
        ///<exception cref="ArgumentNullException">
        ///   <paramref name="options"/> is <see langword="null"/>.
        /// </exception>
        internal CustomFunctionsClient(HttpClient httpClient, DataCoreHttpClientOptions options) : base(httpClient, options) { }


        /// <summary>
        /// Runs a custom function.
        /// </summary>
        /// <typeparam name="T">
        ///   The function return type.
        /// </typeparam>
        /// <param name="url">
        ///   The custom function URL.
        /// </param>
        /// <param name="request">
        ///   The custom function request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A task that will return the custom function result.
        /// </returns>
        internal async Task<T> RunCustomFunctionAsync<T>(
            Uri url,
            CustomFunctionRequest request,
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request), true);

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url) {
                Content = CreateJsonContent(request)
            };

            try {
                using (var response = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                    await response.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await ReadFromJsonAsync<T>(response, cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                httpRequest.Dispose();
            }
        }

    }
}
