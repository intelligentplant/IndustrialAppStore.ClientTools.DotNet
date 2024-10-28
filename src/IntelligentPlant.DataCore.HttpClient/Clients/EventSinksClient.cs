using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using IntelligentPlant.DataCore.Client.Model;
using IntelligentPlant.DataCore.Client.Model.Queries;
using IntelligentPlant.DataCore.Client.Queries;

namespace IntelligentPlant.DataCore.Client.Clients {
    /// <summary>
    /// Client for performing Data Core event source queries.
    /// </summary>
    public class EventSinksClient : ClientBase {

        #region [ Constructor ]

        /// <summary>
        /// Creates a new <see cref="EventSinksClient"/> object.
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
        internal EventSinksClient(HttpClient httpClient, DataCoreHttpClientOptions options) : base(httpClient, options) { }

        #endregion

        #region [ Discovery / Authorization ]

        /// <summary>
        /// Gets information about running event sinks.
        /// </summary>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A task that will return information about the running event sinks.
        /// </returns>
        public async Task<IEnumerable<ComponentInfo>> GetEventSinksAsync(CancellationToken cancellationToken = default) {
            var url = GetAbsoluteUrl("api/data/eventsinks");

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            try {
                using (var response = await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false)) {
                    await response.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await response.Content.ReadAsAsync<IEnumerable<ComponentInfo>>(cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                request.Dispose();
            }
        }


        /// <summary>
        /// Gets information about a running event sink.
        /// </summary>
        /// <param name="eventSinkName">
        ///   The event sink name.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A task that will return information about the event sink.
        /// </returns>
        public async Task<ComponentInfo> GetEventSinkAsync(
            string eventSinkName,
            CancellationToken cancellationToken = default
        ) {
            if (string.IsNullOrWhiteSpace(eventSinkName)) {
                throw new ArgumentException(Resources.Error_EventSinkNameIsRequired, nameof(eventSinkName));
            }

            var url = GetAbsoluteUrl($"api/data/eventsinks/{Uri.EscapeDataString(eventSinkName)}");

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            try {
                using (var response = await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false)) {
                    await response.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await response.Content.ReadAsAsync<ComponentInfo>(cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                request.Dispose();
            }
        }


        /// <summary>
        /// Tests if the user is authorized in any of the specified roles on an event sink.
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
            IsAuthorizedOnEventSinkRequest request,
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

            var url = GetAbsoluteUrl($"api/security/event-sink/{Uri.EscapeDataString(request.EventSinkName)}/is-in-role");

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url) {
                Content = CreateJsonContent(roleNames)
            };

            try {
                using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                    await httpResponse.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await httpResponse.Content.ReadAsAsync<ComponentRoles>(cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                httpRequest.Dispose();
            }
        }

        #endregion

        #region [ Custom Functions ]

        /// <summary>
        /// Runs a custom function on an event sink.
        /// </summary>
        /// <typeparam name="T">
        ///   The return type of the function.
        /// </typeparam>
        /// <param name="eventSinkName">
        ///   The event sink to run the function on.
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
            string eventSinkName, 
            string functionName, 
            IDictionary<string, string>? parameters = null,
            CancellationToken cancellationToken = default
        ) {
            if (string.IsNullOrWhiteSpace(eventSinkName)) {
                throw new ArgumentException(Resources.Error_EventSinkNameIsRequired, nameof(eventSinkName));
            }
            var request = new CustomFunctionRequest() {
                ComponentName = eventSinkName,
                MethodName = functionName,
                Parameters = parameters == null
                    ? new Dictionary<string, string>()
                    : new Dictionary<string, string>(parameters)
            };

            return await CustomFunctionsClient.RunCustomFunctionAsync<T>(
                HttpClient,
                GetAbsoluteUrl("api/rpc")!,
                request,
                cancellationToken
            ).ConfigureAwait(false);
        }

        #endregion

    }
}
