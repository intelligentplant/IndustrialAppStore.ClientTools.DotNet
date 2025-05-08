using System.Net.Http.Formatting;
using System.Net.Http.Json;

using IntelligentPlant.DataCore.Client.Clients;

namespace IntelligentPlant.DataCore.Client {

    /// <summary>
    /// HTTP client for querying Intelligent Plant Data Core and the Industrial App Store for 
    /// real-time, historical, and alarm &amp; event data.
    /// </summary>
    public class DataCoreHttpClient {

        #region [ Fields / Properties ]

        /// <summary>
        /// The HTTP client.
        /// </summary>
        public HttpClient HttpClient { get; }

        /// <summary>
        /// The HTTP client options.
        /// </summary>
        protected DataCoreHttpClientOptions Options { get; }

        /// <summary>
        /// The asset model client.
        /// </summary>
        public AssetModelClient AssetModel { get; }

        /// <summary>
        /// The Data Core information client.
        /// </summary>
        public InfoClient Info { get; }

        /// <summary>
        /// The data sources client.
        /// </summary>
        public DataSourcesClient DataSources { get; }

        /// <summary>
        /// The event sources client.
        /// </summary>
        public EventSourcesClient EventSources { get; }

        /// <summary>
        /// The event sinks client.
        /// </summary>
        public EventSinksClient EventSinks { get; }

        #endregion

        #region [ Constructor ]

        /// <summary>
        /// Creates a new <see cref="DataCoreHttpClient"/> object.
        /// </summary>
        /// <param name="httpClient">
        ///   The HTTP client to use.
        /// </param>
        /// <param name="options">
        ///   The client options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="httpClient"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="options"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   <see cref="DataCoreHttpClientOptions.DataCoreUrl"/> on <paramref name="options"/> is 
        ///   <see langword="null"/>.
        /// </exception>
        public DataCoreHttpClient(HttpClient httpClient, DataCoreHttpClientOptions options) {
            HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            Options = options ?? throw new ArgumentNullException(nameof(options));

            if (Options.DataCoreUrl == null) {
                throw new ArgumentException(Resources.Error_BaseUrlIsRequired, nameof(options));
            }

            HttpClient.BaseAddress = Options.DataCoreUrl.AbsoluteUri.EndsWith("/", StringComparison.Ordinal)
                ? Options.DataCoreUrl
                : new Uri(Options.DataCoreUrl.ToString() + "/");

            AssetModel = new AssetModelClient(HttpClient, Options);
            Info = new InfoClient(HttpClient, Options);
            DataSources = new DataSourcesClient(HttpClient, Options);
            EventSources = new EventSourcesClient(HttpClient, Options);
            EventSinks = new EventSinksClient(HttpClient, Options);
        }

        #endregion

        /// <summary>
        /// Creates an <see cref="HttpContent"/> object that serializes the specified value to JSON.
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
        public HttpContent CreateJsonContent<T>(T value) {
            if (Options.JsonSerializer == JsonSerializerType.SystemTextJson) {
                return JsonContent.Create(value, options: Options.JsonSerializerOptions);
            }

            return new ObjectContent(typeof(T), value, new JsonMediaTypeFormatter());
        }

    }

}
