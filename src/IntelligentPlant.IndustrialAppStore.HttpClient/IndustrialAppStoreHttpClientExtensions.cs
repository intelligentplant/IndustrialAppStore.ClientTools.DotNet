using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using IntelligentPlant.DataCore.Client;
using IntelligentPlant.IndustrialAppStore.Client.Clients;
using IntelligentPlant.IndustrialAppStore.Client.Model;
using IntelligentPlant.IndustrialAppStore.Client.Queries;

namespace IntelligentPlant.IndustrialAppStore.Client {

    /// <summary>
    /// Extensions for calling Industrial App Store API methods.
    /// </summary>
    public static class IndustrialAppStoreHttpClientExtensions {

        /// <summary>
        /// Performs an organisation user search.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The type of the <paramref name="context"/> object.
        /// </typeparam>
        /// <param name="client">
        ///   The <see cref="OrganizationInfoClient{TContext}"/>.
        /// </param>
        /// <param name="nameFilter">
        ///   The name filter to apply to the search.
        /// </param>
        /// <param name="page">
        ///   The results page to retrieve.
        /// </param>
        /// <param name="pageSize">
        ///   The page size for the query.
        /// </param>
        /// <param name="includeExternalResults">
        ///   Specifies if results from trusted external organisations should be included.
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
        ///   <paramref name="client"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ValidationException">
        ///   The query parameters fail validation.
        /// </exception>
        public static async Task<IEnumerable<UserOrGroupPrincipal>> FindUsersAsync<TContext>(
            this OrganizationInfoClient<TContext> client,
            string nameFilter,
            int page = 1,
            int pageSize = 10,
            bool includeExternalResults = false,
            TContext? context = default,
            CancellationToken cancellationToken = default
        ) {
            if (client == null) {
                throw new ArgumentNullException(nameof(client));
            }

            var request = new UserOrGroupPrincipalSearchRequest() { 
                Filter = nameFilter,
                Page = page,
                PageSize = pageSize,
                IncludeExternalResults = includeExternalResults
            };

            return await client.FindUsersAsync(request, context, cancellationToken).ConfigureAwait(false);
        }


        /// <summary>
        /// Performs an organisation group search.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The type of the <paramref name="context"/> object.
        /// </typeparam>
        /// <param name="client">
        ///   The <see cref="OrganizationInfoClient{TContext}"/>.
        /// </param>
        /// <param name="nameFilter">
        ///   The name filter to apply to the search.
        /// </param>
        /// <param name="page">
        ///   The results page to retrieve.
        /// </param>
        /// <param name="pageSize">
        ///   The page size for the query.
        /// </param>
        /// <param name="includeExternalResults">
        ///   Specifies if results from trusted external organisations should be included.
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
        ///   <paramref name="client"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ValidationException">
        ///   The query parameters fail validation.
        /// </exception>
        public static async Task<IEnumerable<UserOrGroupPrincipal>> FindGroupsAsync<TContext>(
            this OrganizationInfoClient<TContext> client,
            string nameFilter,
            int page = 1,
            int pageSize = 10,
            bool includeExternalResults = false,
            TContext? context = default,
            CancellationToken cancellationToken = default
        ) {
            if (client == null) {
                throw new ArgumentNullException(nameof(client));
            }

            var request = new UserOrGroupPrincipalSearchRequest() {
                Filter = nameFilter,
                Page = page,
                PageSize = pageSize,
                IncludeExternalResults = includeExternalResults
            };

            return await client.FindGroupsAsync(request, context, cancellationToken).ConfigureAwait(false);
        }


        /// <summary>
        /// Debits a user for app usage.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The type of the <paramref name="context"/> object.
        /// </typeparam>
        /// <param name="client">
        ///   The <see cref="AccountTransactionsClient{TContext}"/>.
        /// </param>
        /// <param name="amount">
        ///   The number of credits to debit the user. Negative numbers are not allowed.
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
        ///   A <see cref="Task{TResult}"/> that will return a <see cref="DebitUserResponse"/> 
        ///   specifying if the operation was successful.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="client"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ValidationException">
        ///   The query parameters fail validation.
        /// </exception>
        public static async Task<DebitUserResponse> DebitUserAsync<TContext>(
            this AccountTransactionsClient<TContext> client,
            double amount,
            TContext? context = default,
            CancellationToken cancellationToken = default
        ) {
            if (client == null) {
                throw new ArgumentNullException(nameof(client));
            }

            var request = new DebitUserRequest() {
                DebitAmount = amount
            };

            return await client.DebitUserAsync(request, context, cancellationToken).ConfigureAwait(false);
        }


        /// <summary>
        /// Refunds a transaction.
        /// </summary>
        /// <typeparam name="TContext">
        ///   The type of the <paramref name="context"/> object.
        /// </typeparam>
        /// <param name="client">
        ///   The <see cref="AccountTransactionsClient{TContext}"/>.
        /// </param>
        /// <param name="transactionId">
        ///   The ID of the transaction to refund.
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
        ///   A <see cref="Task{TResult}"/> that will return a <see cref="RefundUserResponse"/> 
        ///   specifying if the operation was successful.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="client"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ValidationException">
        ///   The query parameters fail validation.
        /// </exception>
        public static async Task<RefundUserResponse> RefundUserAsync<TContext>(
            this AccountTransactionsClient<TContext> client,
            string transactionId,
            TContext? context = default,
            CancellationToken cancellationToken = default
        ) {
            if (client == null) {
                throw new ArgumentNullException(nameof(client));
            }

            var request = new RefundUserRequest() {
                TransactionRef = transactionId
            };

            return await client.RefundUserAsync(request, context, cancellationToken).ConfigureAwait(false);
        }

    }
}
