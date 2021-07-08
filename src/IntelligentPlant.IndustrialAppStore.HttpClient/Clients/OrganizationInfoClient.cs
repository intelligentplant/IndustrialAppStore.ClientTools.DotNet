using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using IntelligentPlant.DataCore.Client;
using IntelligentPlant.IndustrialAppStore.Client.Model;
using IntelligentPlant.IndustrialAppStore.Client.Queries;

namespace IntelligentPlant.IndustrialAppStore.Client.Clients {

    /// <summary>
    /// Client for querying an Industrial App Store user's organisation.
    /// </summary>
    /// <typeparam name="TContext">
    ///   The context type that is passed to API calls to allow authentication headers to be added 
    ///   to outgoing requests.
    /// </typeparam>
    public class OrganizationInfoClient<TContext> : IasClientBase {

        /// <summary>
        /// Creates a new <see cref="OrganizationInfoClient{TContext}"/> object.
        /// </summary>
        /// <param name="httpClient">
        ///   The HTTP client to use.
        /// </param>
        /// <param name="options">
        ///   The HTTP client options.
        /// </param>
        /// <param name="jsonOptions">
        ///   JSON serializer options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="httpClient"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   <paramref name="options"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   <paramref name="jsonOptions"/> is <see langword="null"/>.
        /// </exception>
        public OrganizationInfoClient(HttpClient httpClient, IndustrialAppStoreHttpClientOptions options, JsonSerializerOptions jsonOptions)
            : base(httpClient, options, jsonOptions) { }


        /// <summary>
        /// Performs a user search.
        /// </summary>
        /// <param name="request">
        ///   The search request.
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
        ///   A <see cref="Task{TResult}"/> that will return the matching users.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="request"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ValidationException">
        ///   <paramref name="request"/> fails validation.
        /// </exception>
        public async Task<IEnumerable<UserOrGroupPrincipal>> FindUsersAsync(
            UserOrGroupPrincipalSearchRequest request,
            TContext context = default,
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request), true);

            var url = GetAbsoluteUrl("api/user-search/users");

            using (var httpRequest = CreateHttpRequestMessage(HttpMethod.Post, url, context, request))
            using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                return await ReadFromJsonAsync<IEnumerable<UserOrGroupPrincipal>>(httpResponse, cancellationToken).ConfigureAwait(false);
            }
        }


        /// <summary>
        /// Performs a group search.
        /// </summary>
        /// <param name="request">
        ///   The search request.
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
        ///   A <see cref="Task{TResult}"/> that will return the matching groups.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="request"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ValidationException">
        ///   <paramref name="request"/> fails validation.
        /// </exception>
        public async Task<IEnumerable<UserOrGroupPrincipal>> FindGroupsAsync(
           UserOrGroupPrincipalSearchRequest request,
           TContext context = default,
           CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request), true);

            var url = GetAbsoluteUrl("api/user-search/groups");

            using (var httpRequest = CreateHttpRequestMessage(HttpMethod.Post, url, context, request))
            using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                return await ReadFromJsonAsync<IEnumerable<UserOrGroupPrincipal>>(httpResponse, cancellationToken).ConfigureAwait(false);
            }
        }


        /// <summary>
        /// Gets the group memberships for the calling user.
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
        ///   A task that will return the matching groups.
        /// </returns>
        public async Task<IEnumerable<UserOrGroupPrincipal>> GetGroupMembershipsAsync(
           TContext context = default,
           CancellationToken cancellationToken = default
        ) {
            var url = GetAbsoluteUrl("api/user-search/me/groups");

            using (var httpRequest = CreateHttpRequestMessage(HttpMethod.Get, url, context))
            using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                return await ReadFromJsonAsync<IEnumerable<UserOrGroupPrincipal>>(httpResponse, cancellationToken).ConfigureAwait(false);
            }
        }

    }
}
