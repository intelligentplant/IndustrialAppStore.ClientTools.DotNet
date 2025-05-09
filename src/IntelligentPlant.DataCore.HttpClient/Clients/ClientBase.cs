using System.Net.Http.Formatting;
using System.Net.Http.Json;
using System.Text.Json;

namespace IntelligentPlant.DataCore.Client.Clients {

    /// <summary>
    /// Base class for <see cref="DataCoreHttpClient"/> sub-client types.
    /// </summary>
    public abstract class ClientBase {

        /// <summary>
        /// The HTTP client to use when making requests.
        /// </summary>
        protected HttpClient HttpClient { get; }

        /// <summary>
        /// The HTTP client options.
        /// </summary>
        protected DataCoreHttpClientOptions Options { get; }

        /// <summary>
        /// The base URL for the client.
        /// </summary>
        protected virtual Uri BaseUrl => Options.DataCoreUrl;


        /// <summary>
        /// Creates a new <see cref="ClientBase"/> instance.
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
        protected ClientBase(HttpClient httpClient, DataCoreHttpClientOptions options) {
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
        protected Uri? GetAbsoluteUrl(string relativeUrl) {
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
        internal static Uri? GetAbsoluteUrl(Uri baseUrl, string relativeUrl) {
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
        /// Creates a new <see cref="HttpContent"/> object that serializes the specified value to 
        /// JSON.
        /// </summary>
        /// <typeparam name="T">
        ///   The value type.
        /// </typeparam>
        /// <param name="value">
        ///   The value to serialize.
        /// </param>
        /// <returns>
        ///   A new <see cref="HttpContent"/> object.
        /// </returns>
        protected internal HttpContent CreateJsonContent<T>(T value) { 
            if (Options.JsonSerializer == JsonSerializerType.SystemTextJson) {
                return JsonContent.Create(value, options: Options.JsonOptions);
            }

            return new ObjectContent(typeof(T), value, new JsonMediaTypeFormatter());
        }


        /// <summary>
        /// Reads the JSON content from the response and deserializes it to the specified type.
        /// </summary>
        /// <typeparam name="T">
        ///   The type to deserialize the JSON content to.
        /// </typeparam>
        /// <param name="response">
        ///   The HTTP response message.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A task that returns the deserialized value.
        /// </returns>
        protected internal async Task<T> ReadFromJsonAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken) {
            if (Options.JsonSerializer == JsonSerializerType.SystemTextJson) {
                return (await response.Content.ReadFromJsonAsync<T>(Options.JsonOptions, cancellationToken).ConfigureAwait(false))!;
            }
            return await response.Content.ReadAsAsync<T>(cancellationToken).ConfigureAwait(false);
        }

    }
}
