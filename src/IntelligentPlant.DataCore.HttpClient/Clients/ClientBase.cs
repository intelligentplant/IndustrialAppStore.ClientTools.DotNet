﻿using System;
using System.Net.Http;

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
        ///   cannot be converted to an absolute URL.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="relativeUrl"/> is <see langword="null"/>.
        /// </exception>
        protected Uri GetAbsoluteUrl(string relativeUrl) {
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

            if (Uri.TryCreate(relativeUrl, UriKind.Absolute, out var result)) {
                return result;
            }

            if (Uri.TryCreate(baseUrl, relativeUrl, out result)) {
                return result;
            }

            return null;
        }

    }
}
