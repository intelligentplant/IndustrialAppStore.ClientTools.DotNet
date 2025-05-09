using System.ComponentModel.DataAnnotations;

using IntelligentPlant.DataCore.Client.Model;
using IntelligentPlant.DataCore.Client.Model.Queries;
using IntelligentPlant.DataCore.Client.Queries;

namespace IntelligentPlant.DataCore.Client.Clients {
    /// <summary>
    /// Client for performing Data Core event source queries.
    /// </summary>
    public class EventSourcesClient : ClientBase {

        #region [ Constructor ]

        /// <summary>
        /// Creates a new <see cref="EventSourcesClient"/> object.
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
        internal EventSourcesClient(HttpClient httpClient, DataCoreHttpClientOptions options) : base(httpClient, options) { }

        #endregion

        #region [ Discovery / Authorization ]

        /// <summary>
        /// Gets information about running event sources.
        /// </summary>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A task that will return information about the running event sources.
        /// </returns>
        public async Task<IEnumerable<ComponentInfo>> GetEventSourcesAsync(CancellationToken cancellationToken = default) {
            var url = GetAbsoluteUrl("api/data/eventsources");

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            try {
                using (var response = await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false)) {
                    await response.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await ReadFromJsonAsync<IEnumerable<ComponentInfo>>(response, cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                request.Dispose();
            }
        }


        /// <summary>
        /// Gets information about a running event source.
        /// </summary>
        /// <param name="eventSourceName">
        ///   The event source name.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A task that will return information about the event source.
        /// </returns>
        public async Task<ComponentInfo> GetEventSourceAsync(
            string eventSourceName,
            CancellationToken cancellationToken = default
        ) {
            if (string.IsNullOrWhiteSpace(eventSourceName)) {
                throw new ArgumentException(Resources.Error_EventSourceNameIsRequired, nameof(eventSourceName));
            }

            var url = GetAbsoluteUrl($"api/data/eventsources/{Uri.EscapeDataString(eventSourceName)}");

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            try {
                using (var response = await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false)) {
                    await response.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await ReadFromJsonAsync<ComponentInfo>(response, cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                request.Dispose();
            }
        }


        /// <summary>
        /// Tests if the user is authorized in any of the specified roles on an event source.
        /// </summary>
        /// <param name="request">
        ///   The request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A task that will return the authorization check results.
        /// </returns>
        public async Task<ComponentRoles> IsAuthorizedAsync(
            IsAuthorizedOnEventSourceRequest request,
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request), true);
            var roleNames = request.RoleNames?.Where(x => !string.IsNullOrWhiteSpace(x))?.Distinct()?.ToArray();

            if (roleNames == null || !roleNames.Any()) {
                throw new ArgumentException(Resources.Error_OneOrMoreRoleNamesRequired, nameof(roleNames));
            }

            var url = GetAbsoluteUrl($"api/security/event-source/{Uri.EscapeDataString(request.EventSourceName)}/is-in-role");

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url) {
                Content = CreateJsonContent(roleNames)
            };

            try {
                using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                    await httpResponse.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await ReadFromJsonAsync<ComponentRoles>(httpResponse, cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                httpRequest.Dispose();
            }
        }

        #endregion

        #region [ Custom Functions ]

        /// <summary>
        /// Runs a custom function on an event source.
        /// </summary>
        /// <typeparam name="T">
        ///   The return type of the function.
        /// </typeparam>
        /// <param name="eventSourceName">
        ///   The event source to run the function on.
        /// </param>
        /// <param name="functionName">
        ///   The function to run.
        /// </param>
        /// <param name="parameters">
        ///   The function parameters.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A task that will return the custom function result.
        /// </returns>
        public async Task<T> RunCustomFunctionAsync<T>(
            string eventSourceName, 
            string functionName, 
            IDictionary<string, string>? parameters = null,
            CancellationToken cancellationToken = default
        ) {
            if (string.IsNullOrWhiteSpace(eventSourceName)) {
                throw new ArgumentException(Resources.Error_EventSourceNameIsRequired, nameof(eventSourceName));
            }
            var request = new CustomFunctionRequest() {
                ComponentName = eventSourceName,
                MethodName = functionName,
                Parameters = parameters == null
                    ? new Dictionary<string, string>()
                    : new Dictionary<string, string>(parameters)
            };

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, GetAbsoluteUrl("api/rpc")!) {
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

        #endregion

    }
}
