using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using IntelligentPlant.DataCore.Client.Clients;

namespace IntelligentPlant.DataCore.Client {

    /// <summary>
    /// HTTP client for querying Intelligent Plant Data Core and the Industrial App Store for 
    /// real-time, historical, and alarm &amp; event data.
    /// </summary>
    /// <typeparam name="TContext">
    ///   The context type that is passed to API calls to allow authentication headers to be added 
    ///   to outgoing requests.
    /// </typeparam>
    /// <typeparam name="TOptions">
    ///   The options type for the client.
    /// </typeparam>
    /// <remarks>
    ///   When querying the Industrial App Store, an <c>Authorization</c> header must be set on 
    ///   every outgoing request. Use the <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/> 
    ///   method to create a message handler to add the the request pipeline for the 
    ///   <see cref="System.Net.Http.HttpClient"/> specified when creating a new 
    ///   <see cref="DataCoreHttpClient{TContext, TOptions}"/> instance, to allow the 
    ///   <see cref="DataCoreHttpClient{TContext, TOptions}"/> to invoke a callback on demand to 
    ///   retrieve the <c>Authorization</c> header to add to outgoing requests.
    /// </remarks>
    public abstract class DataCoreHttpClient<TContext, TOptions> where TOptions : DataCoreHttpClientOptions {

        #region [ Fields / Properties ]

        /// <summary>
        /// The HTTP client.
        /// </summary>
        public HttpClient HttpClient { get; }

        /// <summary>
        /// The HTTP client options.
        /// </summary>
        protected TOptions Options { get; }

        /// <summary>
        /// The asset model client.
        /// </summary>
        public AssetModelClient<TContext, TOptions> AssetModel { get; }

        /// <summary>
        /// The Data Core information client.
        /// </summary>
        public InfoClient<TContext, TOptions> Info { get; }

        /// <summary>
        /// The data sources client.
        /// </summary>
        public DataSourcesClient<TContext, TOptions> DataSources { get; }

        /// <summary>
        /// The event sources client.
        /// </summary>
        public EventSourcesClient<TContext, TOptions> EventSources { get; }

        /// <summary>
        /// The event sinks client.
        /// </summary>
        public EventSinksClient<TContext, TOptions> EventSinks { get; }

        #endregion

        #region [ Constructor ]

        /// <summary>
        /// Creates a new <see cref="DataCoreHttpClient{TContext}"/> object.
        /// </summary>
        /// <param name="httpClient">
        ///   The HTTP client to use. When querying the Industrial App Store, an <c>Authorization</c> 
        ///   header must be set on every outgoing request. Use the <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/> 
        ///   method to create a message handler to add the the request pipeline when creating the 
        ///   <paramref name="httpClient"/>, to allow the <see cref="DataCoreHttpClient{TContext}"/> 
        ///   to invoke a callback on demand to retrieve the <c>Authorization</c> header to add to 
        ///   outgoing requests.
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
        protected DataCoreHttpClient(HttpClient httpClient, TOptions options) {
            HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            Options = options ?? throw new ArgumentNullException(nameof(options));

            if (Options.DataCoreUrl == null) {
                throw new ArgumentException(Resources.Error_BaseUrlIsRequired, nameof(options));
            }

            AssetModel = new AssetModelClient<TContext, TOptions>(HttpClient, Options);
            Info = new InfoClient<TContext, TOptions>(HttpClient, Options);
            DataSources = new DataSourcesClient<TContext, TOptions>(HttpClient, Options);
            EventSources = new EventSourcesClient<TContext, TOptions>(HttpClient, Options);
            EventSinks = new EventSinksClient<TContext, TOptions>(HttpClient, Options);
        }

        #endregion

        #region [ Helper Methods ]

        /// <summary>
        /// Sends an HTTP request using the underlying <see cref="System.Net.Http.HttpClient"/>.
        /// </summary>
        /// <param name="request">
        ///   The request to send.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A task that will return the HTTP response message.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="request"/> is <see langword="null"/>.
        /// </exception>
        /// <remarks>
        ///   Use <see cref="SendAsync(HttpRequestMessage, TContext?, CancellationToken)"/> if the 
        ///   request requires a <typeparamref name="TContext"/> to be set for authentication purposes.
        /// </remarks>
        /// <seealso cref="SendAsync(HttpRequestMessage, TContext?, CancellationToken)"/>
        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default) {
            return await HttpClient.SendAsync(request ?? throw new ArgumentNullException(nameof(request)), cancellationToken).ConfigureAwait(false);
        }


        /// <summary>
        /// Sends an HTTP request using the underlying <see cref="System.Net.Http.HttpClient"/>.
        /// </summary>
        /// <param name="request">
        ///   The request to send.
        /// </param>
        /// <param name="context">
        ///   The <typeparamref name="TContext"/> to associate with the request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A task that will return the HTTP response message.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="request"/> is <see langword="null"/>.
        /// </exception>
        /// <seealso cref="SendAsync(HttpRequestMessage, CancellationToken)"/>
        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, TContext? context = default, CancellationToken cancellationToken = default) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            return await HttpClient.SendAsync(request.AddStateProperty(context), cancellationToken).ConfigureAwait(false);
        }


        /// <summary>
        /// Sends an HTTP GET request using the underlying <see cref="System.Net.Http.HttpClient"/>.
        /// </summary>
        /// <param name="requestUri">
        ///   The request URL.
        /// </param>
        /// <param name="context">
        ///   The <typeparamref name="TContext"/> to associate with the request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A task that will return the HTTP response message.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="requestUri"/> is <see langword="null"/>.
        /// </exception>
        public async Task<HttpResponseMessage> GetAsync(string requestUri, TContext? context = default, CancellationToken cancellationToken = default) {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, requestUri ?? throw new ArgumentNullException(nameof(requestUri)));
            var httpResponse = await SendAsync(httpRequest, context, cancellationToken).ConfigureAwait(false);
            return httpResponse;
        }


        /// <summary>
        /// Sends an HTTP GET request using the underlying <see cref="System.Net.Http.HttpClient"/>.
        /// </summary>
        /// <param name="requestUri">
        ///   The request URL.
        /// </param>
        /// <param name="context">
        ///   The <typeparamref name="TContext"/> to associate with the request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A task that will return the HTTP response message.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="requestUri"/> is <see langword="null"/>.
        /// </exception>
        public async Task<HttpResponseMessage> GetAsync(Uri requestUri, TContext? context = default, CancellationToken cancellationToken = default) {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, requestUri ?? throw new ArgumentNullException(nameof(requestUri)));
            var httpResponse = await SendAsync(httpRequest, context, cancellationToken).ConfigureAwait(false);
            return httpResponse;
        }


        /// <summary>
        /// Sends an HTTP POST request with a JSON-encoded request body using the underlying 
        /// <see cref="System.Net.Http.HttpClient"/>.
        /// </summary>
        /// <param name="requestUri">
        ///   The request URL.
        /// </param>
        /// <param name="request">
        ///   The request object to serialize to JSON.
        /// </param>
        /// <param name="context">
        ///   The <typeparamref name="TContext"/> to associate with the request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A task that will return the HTTP response message.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="requestUri"/> is <see langword="null"/>.
        /// </exception>
        public async Task<HttpResponseMessage> PostAsJsonAsync<TRequest>(string requestUri, TRequest request, TContext? context = default, CancellationToken cancellationToken = default) {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, requestUri ?? throw new ArgumentNullException(nameof(requestUri))) { Content = new ObjectContent(typeof(TRequest), request, new JsonMediaTypeFormatter()) };
            var httpResponse = await SendAsync(httpRequest, context, cancellationToken).ConfigureAwait(false);
            return httpResponse;
        }


        /// <summary>
        /// Sends an HTTP POST request with a JSON-encoded request body using the underlying 
        /// <see cref="System.Net.Http.HttpClient"/>.
        /// </summary>
        /// <param name="requestUri">
        ///   The request URL.
        /// </param>
        /// <param name="request">
        ///   The request object to serialize to JSON.
        /// </param>
        /// <param name="context">
        ///   The <typeparamref name="TContext"/> to associate with the request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A task that will return the HTTP response message.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="requestUri"/> is <see langword="null"/>.
        /// </exception>
        public async Task<HttpResponseMessage> PostAsJsonAsync<TRequest>(Uri requestUri, TRequest request, TContext? context = default, CancellationToken cancellationToken = default) {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, requestUri ?? throw new ArgumentNullException(nameof(requestUri))) { Content = new ObjectContent(typeof(TRequest), request, new JsonMediaTypeFormatter()) };
            var httpResponse = await SendAsync(httpRequest, context, cancellationToken).ConfigureAwait(false);
            return httpResponse;
        }


        /// <summary>
        /// Sends an HTTP PUT request with a JSON-encoded request body using the underlying 
        /// <see cref="System.Net.Http.HttpClient"/>.
        /// </summary>
        /// <param name="requestUri">
        ///   The request URL.
        /// </param>
        /// <param name="request">
        ///   The request object to serialize to JSON.
        /// </param>
        /// <param name="context">
        ///   The <typeparamref name="TContext"/> to associate with the request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A task that will return the HTTP response message.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="requestUri"/> is <see langword="null"/>.
        /// </exception>
        public async Task<HttpResponseMessage> PutAsJsonAsync<TRequest>(string requestUri, TRequest request, TContext? context = default, CancellationToken cancellationToken = default) {
            var httpRequest = new HttpRequestMessage(HttpMethod.Put, requestUri ?? throw new ArgumentNullException(nameof(requestUri))) { Content = new ObjectContent(typeof(TRequest), request, new JsonMediaTypeFormatter()) };
            var httpResponse = await SendAsync(httpRequest, context, cancellationToken).ConfigureAwait(false);
            return httpResponse;
        }


        /// <summary>
        /// Sends an HTTP PUT request with a JSON-encoded request body using the underlying 
        /// <see cref="System.Net.Http.HttpClient"/>.
        /// </summary>
        /// <param name="requestUri">
        ///   The request URL.
        /// </param>
        /// <param name="request">
        ///   The request object to serialize to JSON.
        /// </param>
        /// <param name="context">
        ///   The <typeparamref name="TContext"/> to associate with the request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A task that will return the HTTP response message.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="requestUri"/> is <see langword="null"/>.
        /// </exception>
        public async Task<HttpResponseMessage> PutAsJsonAsync<TRequest>(Uri requestUri, TRequest request, TContext? context = default, CancellationToken cancellationToken = default) {
            var httpRequest = new HttpRequestMessage(HttpMethod.Put, requestUri ?? throw new ArgumentNullException(nameof(requestUri))) { Content = new ObjectContent(typeof(TRequest), request, new JsonMediaTypeFormatter()) };
            var httpResponse = await SendAsync(httpRequest, context, cancellationToken).ConfigureAwait(false);
            return httpResponse;
        }


        /// <summary>
        /// Sends an HTTP DELETE request using the underlying <see cref="System.Net.Http.HttpClient"/>.
        /// </summary>
        /// <param name="requestUri">
        ///   The request URL.
        /// </param>
        /// <param name="context">
        ///   The <typeparamref name="TContext"/> to associate with the request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A task that will return the HTTP response message.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="requestUri"/> is <see langword="null"/>.
        /// </exception>
        public async Task<HttpResponseMessage> DeleteAsync<TRequest>(string requestUri, TContext? context = default, CancellationToken cancellationToken = default) {
            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, requestUri ?? throw new ArgumentNullException(nameof(requestUri)));
            var httpResponse = await SendAsync(httpRequest, context, cancellationToken).ConfigureAwait(false);
            return httpResponse;
        }


        /// <summary>
        /// Sends an HTTP DELETE request using the underlying <see cref="System.Net.Http.HttpClient"/>.
        /// </summary>
        /// <param name="requestUri">
        ///   The request URL.
        /// </param>
        /// <param name="context">
        ///   The <typeparamref name="TContext"/> to associate with the request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A task that will return the HTTP response message.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="requestUri"/> is <see langword="null"/>.
        /// </exception>
        public async Task<HttpResponseMessage> DeleteAsync<TRequest>(Uri requestUri, TContext? context = default, CancellationToken cancellationToken = default) {
            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, requestUri ?? throw new ArgumentNullException(nameof(requestUri)));
            var httpResponse = await SendAsync(httpRequest, context, cancellationToken).ConfigureAwait(false);
            return httpResponse;
        }

        #endregion

    }


    /// <summary>
    /// <see cref="DataCoreHttpClient{TContext, TOptions}"/> implementation that uses 
    /// <see cref="DataCoreHttpClientOptions"/> to describe the client options.
    /// </summary>
    /// <typeparam name="TContext">
    ///   The context type that is passed to API calls to allow authentication headers to be added 
    ///   to outgoing requests.
    /// </typeparam>
    public class DataCoreHttpClient<TContext> : DataCoreHttpClient<TContext, DataCoreHttpClientOptions> {

        /// <summary>
        /// Creates a new <see cref="DataCoreHttpClient{TContext}"/> object.
        /// </summary>
        /// <param name="httpClient">
        ///   The HTTP client to use. When querying the Industrial App Store, an <c>Authorization</c> 
        ///   header must be set on every outgoing request. Use the <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/> 
        ///   method to create a message handler to add the the request pipeline when creating the 
        ///   <paramref name="httpClient"/>, to allow the <see cref="DataCoreHttpClient{TContext}"/> 
        ///   to invoke a callback on demand to retrieve the <c>Authorization</c> header to add to 
        ///   outgoing requests.
        /// </param>
        /// <param name="options">
        ///   The client options.
        /// </param>
        public DataCoreHttpClient(HttpClient httpClient, DataCoreHttpClientOptions options)
            : base(httpClient, options) { }

    }


    /// <summary>
    /// <see cref="DataCoreHttpClient{TContext}"/> that uses an <see cref="object"/> as the 
    /// context associated with API operations.
    /// </summary>
    public class DataCoreHttpClient : DataCoreHttpClient<object> {

        /// <summary>
        /// Creates a new <see cref="DataCoreHttpClient"/> object.
        /// </summary>
        /// <param name="httpClient">
        ///   The HTTP client to use. When querying the Industrial App Store, an <c>Authorization</c> 
        ///   header must be set on every outgoing request. Use the <see cref="CreateAuthenticationMessageHandler"/> 
        ///   method to create a message handler to add the the request pipeline when creating the 
        ///   <paramref name="httpClient"/>, to allow the <see cref="DataCoreHttpClient"/> to 
        ///   invoke a callback on demand to retrieve the <c>Authorization</c> header to add to 
        ///   outgoing requests.
        /// </param>
        /// <param name="options">
        ///   The client options.
        /// </param>
        public DataCoreHttpClient(HttpClient httpClient, DataCoreHttpClientOptions options) 
            : base(httpClient, options) { }


        #region [ Authentication Handler Creation ]

        /// <summary>
        /// Creates a <see cref="DelegatingHandler"/> that can be added to an <see cref="HttpClient"/> 
        /// message pipeline, that will set the <c>Authorize</c> header on outgoing requests based 
        /// on the auth state object passed to a <see cref="DataCoreHttpClient{TContext}"/> method.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The context type that is passed to API calls to allow authentication headers to be added 
        ///   to outgoing requests.
        /// </typeparam>
        /// <param name="callback">
        ///   The callback delegate that will receive the HTTP request and the auth state object 
        ///   and return the <c>Authorize</c> header value to add to the request.
        /// </param>
        /// <returns>
        ///   A new <see cref="DelegatingHandler"/> object.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="callback"/> is <see langword="null"/>.
        /// </exception>
        public static DelegatingHandler CreateAuthenticationMessageHandler<TContext>(AuthenticationCallback<TContext> callback) {
            if (callback == null) {
                throw new ArgumentNullException(nameof(callback));
            }

            async Task ApplyAuthHeaderAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
                var authHeader = await callback.Invoke(request, request.GetStateProperty<TContext>(), cancellationToken).ConfigureAwait(false);
                if (authHeader != null) {
                    request.Headers.Authorization = authHeader;
                }
            }

            return new Jaahas.Http.HttpRequestPipelineHandler(ApplyAuthHeaderAsync);
        }

        #endregion

    }
}
