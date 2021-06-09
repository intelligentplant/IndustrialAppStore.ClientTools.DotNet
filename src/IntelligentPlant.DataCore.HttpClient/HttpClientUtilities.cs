using System;
using System.Net.Http;
using System.Text.Json;

namespace IntelligentPlant.DataCore.Client {

    /// <summary>
    /// Utilities for creating <see cref="HttpRequestMessage"/> instances.
    /// </summary>
    public static class HttpClientUtilities {

        /// <summary>
        /// Creates a new <see cref="HttpRequestMessage"/> that has the specified context attached.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The context type for the request.
        /// </typeparam>
        /// <param name="method">
        ///   The request method.
        /// </param>
        /// <param name="url">
        ///   The request URL.
        /// </param>
        /// <param name="requestContext">
        ///   The request context.
        /// </param>
        /// <returns>
        ///   A new <see cref="HttpRequestMessage"/> object.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="method"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="url"/> is <see langword="null"/>.
        /// </exception>
        public static HttpRequestMessage CreateHttpRequestMessage<TContext>(
            HttpMethod method,
            Uri url,
            TContext requestContext
        ) {
            return new HttpRequestMessage(
                method ?? throw new ArgumentNullException(nameof(method)),
                url ?? throw new ArgumentNullException(nameof(url))
            ).AddStateProperty(requestContext);
        }


        /// <summary>
        /// Creates a new <see cref="HttpRequestMessage"/> that has the specified metadata attached.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The context type for the request.
        /// </typeparam>
        /// <param name="method">
        ///   The request method.
        /// </param>
        /// <param name="url">
        ///   The request URL.
        /// </param>
        /// <param name="requestContext">
        ///   The request context.
        /// </param>
        /// <returns>
        ///   A new <see cref="HttpRequestMessage"/> object.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="method"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="url"/> is <see langword="null"/>.
        /// </exception>
        public static HttpRequestMessage CreateHttpRequestMessage<TContext>(
            HttpMethod method,
            string url,
            TContext requestContext
        ) {
            return CreateHttpRequestMessage(
                method,
                new Uri(url ?? throw new ArgumentNullException(nameof(url)), UriKind.RelativeOrAbsolute),
                requestContext
            );
        }


        /// <summary>
        /// Creates a new <see cref="HttpRequestMessage"/> that has the specified content and 
        /// request context attached.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The context type for the request.
        /// </typeparam>
        /// <typeparam name="TContent">
        ///   The type of the <paramref name="content"/>.
        /// </typeparam>
        /// <param name="method">
        ///   The request method.
        /// </param>
        /// <param name="url">
        ///   The request URL.
        /// </param>
        /// <param name="content">
        ///   The content for the request. The content will be serialized to JSON.
        /// </param>
        /// <param name="requestContext">
        ///   The request context.
        /// </param>
        /// <param name="options">
        ///   The JSON serializer options to use.
        /// </param>
        /// <returns>
        ///   A new <see cref="HttpRequestMessage"/> object.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="method"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="url"/> is <see langword="null"/>.
        /// </exception>
        public static HttpRequestMessage CreateHttpRequestMessage<TContext, TContent>(
            HttpMethod method,
            Uri url,
            TContext requestContext,
            TContent content,
            JsonSerializerOptions options
        ) {
            var result = CreateHttpRequestMessage(method, url, requestContext);
            result.Content = System.Net.Http.Json.JsonContent.Create(content, options: options);

            return result;
        }



        /// <summary>
        /// Creates a new <see cref="HttpRequestMessage"/> that has the specified content and 
        /// request context attached.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The context type for the request.
        /// </typeparam>
        /// <typeparam name="TContent">
        ///   The type of the <paramref name="content"/>.
        /// </typeparam>
        /// <param name="method">
        ///   The request method.
        /// </param>
        /// <param name="url">
        ///   The request URL.
        /// </param>
        /// <param name="content">
        ///   The content for the request. The content will be serialized to JSON.
        /// </param>
        /// <param name="requestContext">
        ///   The request context.
        /// </param>
        /// <param name="options">
        ///   The JSON serializer options to use.
        /// </param>
        /// <returns>
        ///   A new <see cref="HttpRequestMessage"/> object.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="method"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="url"/> is <see langword="null"/>.
        /// </exception>
        public static HttpRequestMessage CreateHttpRequestMessage<TContext, TContent>(
            HttpMethod method,
            string url,
            TContext requestContext,
            TContent content,
            JsonSerializerOptions options
        ) {
            return CreateHttpRequestMessage(
                method,
                new Uri(url ?? throw new ArgumentNullException(nameof(url)), UriKind.RelativeOrAbsolute),
                requestContext,
                content,
                options
            );
        }

    }
}
