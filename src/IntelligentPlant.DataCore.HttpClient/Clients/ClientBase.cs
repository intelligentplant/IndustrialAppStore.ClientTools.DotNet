using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace IntelligentPlant.DataCore.Client.Clients {

    /// <summary>
    /// Base class for <see cref="DataCoreHttpClient"/> sub-client types.
    /// </summary>
    /// <typeparam name="TOptions">
    ///   The HTTP client options type.
    /// </typeparam>
    public abstract class ClientBase<TOptions> where TOptions : DataCoreHttpClientOptions {

        /// <summary>
        /// The HTTP client to use when making requests.
        /// </summary>
        protected HttpClient HttpClient { get; }

        /// <summary>
        /// The HTTP client options.
        /// </summary>
        protected TOptions Options { get; }

        /// <summary>
        /// The base URL for the client.
        /// </summary>
        protected virtual Uri BaseUrl => Options.DataCoreUrl;


        /// <summary>
        /// Creates a new <see cref="ClientBase{TOptions}"/> object.
        /// </summary>
        /// <param name="httpClient">
        ///   The HTTP client to use.
        /// </param>
        /// <param name="options">>
        ///   The HTTP client options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="httpClient"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="options"/> is <see langword="null"/>.
        /// </exception>
        protected ClientBase(HttpClient httpClient, TOptions options) {
            HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }


        /// <summary>
        /// Converts a relative URL to an absolute URL, using the <see cref="BaseUrl"/> property 
        /// as the base.
        /// </summary>
        /// <param name="relativeUrl">
        ///   The relative URL.
        /// </param>
        /// <returns>
        ///   The absolute URL, or <see langword="null"/> if the <paramref name="relativeUrl"/> 
        ///   cannot be converted to an absolute URL. If <paramref name="relativeUrl"/> is already 
        ///   absolute, it will be returned unmodified.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="relativeUrl"/> is <see langword="null"/>.
        /// </exception>
        protected Uri GetAbsoluteUrl(string relativeUrl) {
            return GetAbsoluteUrl(BaseUrl, relativeUrl);
        }


        /// <summary>
        /// Converts a relative URL to an absolute URL, using the <see cref="BaseUrl"/> property 
        /// as the base.
        /// </summary>
        /// <param name="relativeUrl">
        ///   The relative URL.
        /// </param>
        /// <returns>
        ///   The absolute URL. If <paramref name="relativeUrl"/> is already absolute, it will be 
        ///   returned unmodified.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="relativeUrl"/> is <see langword="null"/>.
        /// </exception>
        protected Uri GetAbsoluteUrl(Uri relativeUrl) {
            return GetAbsoluteUrl(BaseUrl, relativeUrl);
        }


        /// <summary>
        /// Converts a relative URL to an absolute URL.
        /// </summary>
        /// <param name="baseUrl">
        ///   The base URL.
        /// </param>
        /// <param name="relativeUrl">
        ///   The relative URL.
        /// </param>
        /// <returns>
        ///   The absolute URL, or <see langword="null"/> if the <paramref name="relativeUrl"/> 
        ///   cannot be converted to an absolute URL.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="baseUrl"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="relativeUrl"/> is <see langword="null"/>.
        /// </exception>
        internal static Uri GetAbsoluteUrl(Uri baseUrl, string relativeUrl) {
            if (baseUrl == null) {
                throw new ArgumentNullException(nameof(baseUrl));
            }
            if (relativeUrl == null) {
                throw new ArgumentNullException(nameof(relativeUrl));
            }

            if (!Uri.TryCreate(relativeUrl, UriKind.RelativeOrAbsolute, out var parsedUrl)) {
                return null;
            }

            return GetAbsoluteUrl(baseUrl, parsedUrl);
        }


        /// <summary>
        /// Converts a relative URL to an absolute URL.
        /// </summary>
        /// <param name="baseUrl">
        ///   The base URL.
        /// </param>
        /// <param name="relativeUrl">
        ///   The relative URL.
        /// </param>
        /// <returns>
        ///   The absolute URL, or <see langword="null"/> if the <paramref name="relativeUrl"/> 
        ///   cannot be converted to an absolute URL.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="baseUrl"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="relativeUrl"/> is <see langword="null"/>.
        /// </exception>
        internal static Uri GetAbsoluteUrl(Uri baseUrl, Uri relativeUrl) {
            if (baseUrl == null) {
                throw new ArgumentNullException(nameof(baseUrl));
            }
            if (relativeUrl == null) {
                throw new ArgumentNullException(nameof(relativeUrl));
            }

            if (relativeUrl.IsAbsoluteUri) {
                return relativeUrl;
            }

            return new Uri(baseUrl, relativeUrl);
        }


        /// <summary>
        /// Creates an HTTP request message with no body content.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The request context type.
        /// </typeparam>
        /// <param name="method">
        ///   The HTTP method.
        /// </param>
        /// <param name="url">
        ///   The request URI.
        /// </param>
        /// <param name="requestContext">
        ///   The request context.
        /// </param>
        /// <returns>
        ///   A new <see cref="HttpRequestMessage"/> instance.
        /// </returns>
        protected internal static HttpRequestMessage CreateHttpRequestMessage<TContext>(HttpMethod method, Uri url, TContext requestContext) {
            return HttpClientUtilities.CreateHttpRequestMessage(method, url, requestContext);
        }


        /// <summary>
        /// Creates an HTTP request message with the specified body content.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The request context type.
        /// </typeparam>
        /// <typeparam name="TContent">
        ///   The request content type.
        /// </typeparam>
        /// <param name="method">
        ///   The HTTP method.
        /// </param>
        /// <param name="url">
        ///   The request URI.
        /// </param>
        /// <param name="requestContext">
        ///   The request context.
        /// </param>
        /// <param name="content">
        ///   The request content. The content will be serialized to JSON using <see cref="JsonSerializer"/>.
        /// </param>
        /// <param name="jsonOptions">
        ///   The options for <see cref="JsonSerializer"/>.
        /// </param>
        /// <returns>
        ///   A new <see cref="HttpRequestMessage"/> instance.
        /// </returns>
        protected internal static HttpRequestMessage CreateHttpRequestMessage<TContext, TContent>(HttpMethod method, Uri url, TContext requestContext, TContent content, JsonSerializerOptions jsonOptions) {
            return HttpClientUtilities.CreateHttpRequestMessage(method, url, requestContext, content, jsonOptions);
        }


        /// <summary>
        /// Creates an HTTP request message with the specified body content.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The request context type.
        /// </typeparam>
        /// <typeparam name="TContent">
        ///   The request content type.
        /// </typeparam>
        /// <param name="method">
        ///   The HTTP method.
        /// </param>
        /// <param name="url">
        ///   The request URI.
        /// </param>
        /// <param name="requestContext">
        ///   The request context.
        /// </param>
        /// <param name="content">
        ///   The request content. The content will be serialized to JSON using <see cref="JsonSerializer"/>.
        /// </param>
        /// <returns>
        ///   A new <see cref="HttpRequestMessage"/> instance.
        /// </returns>
        protected internal HttpRequestMessage CreateHttpRequestMessage<TContext, TContent>(HttpMethod method, Uri url, TContext requestContext, TContent content) {
            return CreateHttpRequestMessage(method, url, requestContext, content, Options.JsonOptions);
        }


        /// <summary>
        /// Ensures that an HTTP response has a valid status code.
        /// </summary>
        /// <param name="response">
        ///   The HTTP response message.
        /// </param>
        /// <returns>
        ///   A <see cref="Task"/> that will verify the response and throw a <see cref="DataCoreHttpClientException"/> 
        ///   if a non-good status code is returned.
        /// </returns>
        /// <exception cref="DataCoreHttpClientException">
        ///   <paramref name="response"/> contains a non-good status code.
        /// </exception>
        /// <remarks>
        ///   If the <paramref name="response"/> contains an RFC 7807 problem details object, this 
        ///   will be deserialized and assigned to the <see cref="DataCoreHttpClientException.ProblemDetails"/> 
        ///   property on the thrown exception.
        /// </remarks>
        protected internal static async Task VerifyResponseAsync(HttpResponseMessage response) {
            await response.ThrowOnErrorResponse().ConfigureAwait(false);
        }


        /// <summary>
        /// Deserializes the JSON payload in the specified HTTP response using <see cref="JsonSerializer"/>.
        /// </summary>
        /// <typeparam name="T">
        ///   The type to deserialize the JSON response to.
        /// </typeparam>
        /// <param name="response">
        ///   The HTTP response message.
        /// </param>
        /// <param name="jsonOptions">
        ///   The options for <see cref="JsonSerializer"/>.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A <see cref="Task{TResult}"/> that will return the deserialized response.
        /// </returns>
        protected internal static async Task<T> ReadFromJsonAsync<T>(HttpResponseMessage response, JsonSerializerOptions jsonOptions, CancellationToken cancellationToken) {
            await VerifyResponseAsync(response).ConfigureAwait(false);
            return await response.Content.ReadFromJsonAsync<T>(jsonOptions, cancellationToken).ConfigureAwait(false);
        }


        /// <summary>
        /// Deserializes the JSON payload in the specified HTTP response using <see cref="JsonSerializer"/>.
        /// </summary>
        /// <typeparam name="T">
        ///   The type to deserialize the JSON response to.
        /// </typeparam>
        /// <param name="response">
        ///   The HTTP response message.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A <see cref="Task{TResult}"/> that will return the deserialized response.
        /// </returns>
        protected internal Task<T> ReadFromJsonAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken) {
            return ReadFromJsonAsync<T>(response, Options.JsonOptions, cancellationToken);
        }

    }
}
