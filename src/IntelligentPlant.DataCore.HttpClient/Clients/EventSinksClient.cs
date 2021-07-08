﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using IntelligentPlant.DataCore.Client.Queries;
using IntelligentPlant.DataCore.Client.Model;
using IntelligentPlant.DataCore.Client.Model.Queries;
using System.Text.Json;

namespace IntelligentPlant.DataCore.Client.Clients {
    /// <summary>
    /// Client for performing Data Core event source queries.
    /// </summary>
    /// <typeparam name="TContext">
    ///   The context type that is passed to API calls to allow authentication headers to be added 
    ///   to outgoing requests.
    /// </typeparam>
    /// <typeparam name="TOptions">
    ///   The HTTP client options type.
    /// </typeparam>
    public class EventSinksClient<TContext, TOptions> : ClientBase<TOptions> where TOptions : DataCoreHttpClientOptions {

        #region [ Fields / Properties ]

        /// <summary>
        /// Client for calling custom event sink functions.
        /// </summary>
        private readonly CustomFunctionsClient<TContext, TOptions> _customFunctionsClient;

        #endregion

        #region [ Constructor ]

        /// <summary>
        /// Creates a new <see cref="EventSinksClient{TContext, TOptions}"/> object.
        /// </summary>
        /// <param name="httpClient">
        ///   The HTTP client to use.
        /// </param>
        /// <param name="options">
        ///   The HTTP client options to use.
        /// </param>
        /// <param name="jsonOptions">
        ///   JSON serializer options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="httpClient"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="options"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="jsonOptions"/> is <see langword="null"/>.
        /// </exception>
        public EventSinksClient(HttpClient httpClient, TOptions options, JsonSerializerOptions jsonOptions) 
            : base(httpClient, options, jsonOptions) {
            _customFunctionsClient = new CustomFunctionsClient<TContext, TOptions>(httpClient, options, jsonOptions);
        }

        #endregion

        #region [ Discovery / Authorization ]

        /// <summary>
        /// Gets information about running event sinks.
        /// </summary>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, this will be 
        ///   passed to the handler's callback when requesting the <c>Authorize</c> header value 
        ///   for the outgoing request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A task that will return information about the running event sinks.
        /// </returns>
        public async Task<IEnumerable<ComponentInfo>> GetEventSinksAsync(
            TContext context = default, 
            CancellationToken cancellationToken = default
        ) {
            var url = GetAbsoluteUrl("api/data/eventsinks");

            using (var httpRequest = CreateHttpRequestMessage(HttpMethod.Get, url, context))
            using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                return await ReadFromJsonAsync<IEnumerable<ComponentInfo>>(httpResponse, cancellationToken).ConfigureAwait(false);
            }
        }


        /// <summary>
        /// Gets information about a running event sink.
        /// </summary>
        /// <param name="eventSinkName">
        ///   The event sink name.
        /// </param>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, this will be 
        ///   passed to the handler's callback when requesting the <c>Authorize</c> header value 
        ///   for the outgoing request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A task that will return information about the event sink.
        /// </returns>
        public async Task<ComponentInfo> GetEventSinkAsync(
            string eventSinkName,
            TContext context = default, 
            CancellationToken cancellationToken = default
        ) {
            if (string.IsNullOrWhiteSpace(eventSinkName)) {
                throw new ArgumentException(Resources.Error_EventSinkNameIsRequired, nameof(eventSinkName));
            }

            var url = GetAbsoluteUrl($"api/data/eventsinks/{Uri.EscapeDataString(eventSinkName)}");

            using (var httpRequest = CreateHttpRequestMessage(HttpMethod.Get, url, context))
            using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                return await ReadFromJsonAsync<ComponentInfo>(httpResponse, cancellationToken).ConfigureAwait(false);
            }
        }


        /// <summary>
        /// Tests if the user is authorized in any of the specified roles on an event sink.
        /// </summary>
        /// <param name="request">
        ///   The request.
        /// </param>
        /// <param name="context">
        ///   The context for the operation. If the request pipeline contains a handler created 
        ///   via <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/>, this will be 
        ///   passed to the handler's callback when requesting the <c>Authorize</c> header value 
        ///   for the outgoing request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A task that will return the authorization check results.
        /// </returns>
        public async Task<ComponentRoles> IsAuthorizedAsync(
            IsAuthorizedOnEventSinkRequest request,
            TContext context = default, 
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

            using (var httpRequest = CreateHttpRequestMessage(HttpMethod.Post, url, context, roleNames))
            using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                return await ReadFromJsonAsync<ComponentRoles>(httpResponse, cancellationToken).ConfigureAwait(false);
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
        /// <param name="context">
        ///   The auth state for the operation. If the request pipeline contains a handler created 
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
        public async Task<T> RunCustomFunctionAsync<T>(
            string eventSinkName, 
            string functionName, 
            IDictionary<string, string> parameters = null,
            TContext context = default, 
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

            return await _customFunctionsClient.RunCustomFunctionAsync<T>(
                request,
                context,
                cancellationToken
            ).ConfigureAwait(false);
        }

        #endregion

    }
}
