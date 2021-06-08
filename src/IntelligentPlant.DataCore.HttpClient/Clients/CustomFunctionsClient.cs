using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using IntelligentPlant.DataCore.Client.Model.Queries;

namespace IntelligentPlant.DataCore.Client.Clients {

    /// <summary>
    /// Client for performing custom function calls.
    /// </summary>
    /// <typeparam name="TContext">
    ///   The context type that is passed to API calls to allow authentication headers to be added 
    ///   to outgoing requests.
    /// </typeparam>
    /// <typeparam name="TOptions">
    ///   The HTTP client options type.
    /// </typeparam>
    internal class CustomFunctionsClient<TContext, TOptions> : ClientBase<TOptions> where TOptions : DataCoreHttpClientOptions {

        /// <summary>
        /// Creates a new <see cref="CustomFunctionsClient{TContext, TOptions}"/> object.
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
        public CustomFunctionsClient(HttpClient httpClient, TOptions options) : base(httpClient, options) { }


        /// <summary>
        /// Runs a custom function.
        /// </summary>
        /// <typeparam name="T">
        ///   The function return type.
        /// </typeparam>
        /// <param name="httpClient">
        ///   The HTTP client to use.
        /// </param>
        /// <param name="url">
        ///   The custom function URL.
        /// </param>
        /// <param name="request">
        ///   The custom function request.
        /// </param>
        /// <param name="options">
        ///   The Data Core client options.
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
        internal static async Task<T> RunCustomFunctionAsync<T>(
            HttpClient httpClient, 
            Uri url,
            CustomFunctionRequest request,
            TOptions options,
            TContext context = default,
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request), true);

            using (var httpRequest = CreateHttpRequestMessage(HttpMethod.Post, url, context, request, options?.JsonOptions))
            using (var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                return await ReadFromJsonAsync<T>(httpResponse, options?.JsonOptions, cancellationToken).ConfigureAwait(false);
            }
        }

    }
}
