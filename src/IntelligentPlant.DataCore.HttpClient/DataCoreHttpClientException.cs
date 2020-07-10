using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace IntelligentPlant.DataCore.Client {

    /// <summary>
    /// Exception raised when a Data Core HTTP request returns a result with a non-good status code.
    /// </summary>
    public class DataCoreHttpClientException : HttpRequestException {

        /// <summary>
        /// Gets the HTTP verb that was used in the request.
        /// </summary>
        public string Verb { get; }

        /// <summary>
        /// Gets the URL that was used in the request.
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// Gets the HTTP status code that was returned.
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// The HTTP request headers.
        /// </summary>
        public IDictionary<string, string[]> RequestHeaders { get; }

        /// <summary>
        /// The HTTP response headers.
        /// </summary>
        public IDictionary<string, string[]> ResponseHeaders { get; }

        /// <summary>
        /// Gets the response content that was returned
        /// </summary>
        public string Content { get; }


        /// <summary>
        /// Creates a new <see cref="DataCoreHttpClientException"/> object.
        /// </summary>
        /// <param name="message">
        ///   The error message.
        /// </param>
        /// <param name="verb">
        ///   The HTTP verb.
        /// </param>
        /// <param name="url">
        ///   The request URL.
        /// </param>
        /// <param name="statusCode">
        ///   The HTTP status code.
        /// </param>
        /// <param name="requestHeaders">
        ///   The HTTP request headers.
        /// </param>
        /// <param name="responseHeaders">
        ///   The HTTP response headers.
        /// </param>
        /// <param name="responseContent">
        ///   The response content.
        /// </param>
        public DataCoreHttpClientException(string message, string verb, string url, HttpStatusCode statusCode, IDictionary<string, string[]> requestHeaders, IDictionary<string, string[]> responseHeaders, string responseContent) 
            : this(message, verb, url, statusCode, requestHeaders, responseHeaders, responseContent, null) { }


        /// <summary>
        /// Creates a new <see cref="DataCoreHttpClientException"/> object with an inner exception.
        /// </summary>
        /// <param name="message">
        ///   The error message.
        /// </param>
        /// <param name="verb">
        ///   The HTTP verb.
        /// </param>
        /// <param name="url">
        ///   The request URL.
        /// </param>
        /// <param name="statusCode">
        ///   The HTTP status code.
        /// </param>
        /// <param name="requestHeaders">
        ///   The HTTP request headers.
        /// </param>
        /// <param name="responseHeaders">
        ///   The HTTP response headers.
        /// </param>
        /// <param name="responseContent">
        ///   The response content.
        /// </param>
        /// <param name="inner">
        ///   The inner exception.
        /// </param>
        public DataCoreHttpClientException(string message, string verb, string url, HttpStatusCode statusCode, IDictionary<string, string[]> requestHeaders, IDictionary<string, string[]> responseHeaders, string responseContent, Exception inner) 
            : base(message, inner) {
            Verb = verb;
            Url = url;
            StatusCode = statusCode;
            RequestHeaders = new ReadOnlyDictionary<string, string[]>(requestHeaders ?? new Dictionary<string, string[]>());
            ResponseHeaders = new ReadOnlyDictionary<string, string[]>(responseHeaders ?? new Dictionary<string, string[]>());
            Content = responseContent;
        }


        /// <summary>
        /// Creates an <see cref="DataCoreHttpClientException"/> using the specified HTTP response message.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="response">The HTTP response.</param>
        /// <returns>
        /// A task that will return the exception.
        /// </returns>
        internal static Task<DataCoreHttpClientException> FromHttpResponseMessage(string errorMessage, HttpResponseMessage response) {
            return FromHttpResponseMessage(errorMessage, response, null);
        }


        /// <summary>
        /// Creates an <see cref="DataCoreHttpClientException"/> with an inner exception, using the specified HTTP response message.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="response">The HTTP response.</param>
        /// <param name="inner">The inner exception</param>
        /// <returns>
        /// A task that will return the exception.
        /// </returns>
        internal static async Task<DataCoreHttpClientException> FromHttpResponseMessage(string errorMessage, HttpResponseMessage response, Exception inner) {
            if (response == null) {
                throw new ArgumentNullException(nameof(response));
            }

            var requestHeaders = response.RequestMessage.Content == null
                ? response.RequestMessage.Headers.ToDictionary(x => x.Key, x => x.Value.ToArray())
                : response.RequestMessage.Headers.Concat(response.RequestMessage.Content.Headers).ToDictionary(x => x.Key, x => x.Value.ToArray());

            var responseHeaders = response.Content == null
                ? response.Headers.ToDictionary(x => x.Key, x => x.Value.ToArray())
                : response.Headers.Concat(response.Content.Headers).ToDictionary(x => x.Key, x => x.Value.ToArray());

            var includeContent = responseHeaders.TryGetValue("Content-Type", out var contentTypes) && contentTypes != null
                ? contentTypes.Any(x => CanIncludeContent(x))
                : false;

            var content = includeContent
                ? await response.Content.ReadAsStringAsync().ConfigureAwait(false)
                : null;

            return new DataCoreHttpClientException(
                errorMessage, 
                response.RequestMessage.Method.Method, 
                response.RequestMessage.RequestUri.ToString(), 
                response.StatusCode, 
                requestHeaders,
                responseHeaders,
                content, 
                inner
            );
        }


        /// <summary>
        /// Tests if the content of an HTTP response can be included in a 
        /// <see cref="DataCoreHttpClientException"/>.
        /// </summary>
        /// <param name="contentType">
        ///   The content type of the response.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the content can be included; otherwise, 
        ///   <see langword="false"/>.
        /// </returns>
        private static bool CanIncludeContent(string contentType) {
            if (string.IsNullOrWhiteSpace(contentType)) {
                return false;
            }
            if (contentType.StartsWith("application/json")) {
                return true;
            }
            if (contentType.StartsWith("text/")) {
                return true;
            }

            return false;
        }

    }

}
