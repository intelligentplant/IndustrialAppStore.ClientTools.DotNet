using System.ComponentModel.DataAnnotations;

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
        /// <param name="client">
        ///   The <see cref="OrganizationInfoClient"/>.
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
        public static async Task<IEnumerable<UserOrGroupPrincipal>> FindUsersAsync(
            this OrganizationInfoClient client,
            string nameFilter,
            int page = 1,
            int pageSize = 10,
            bool includeExternalResults = false,
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

            return await client.FindUsersAsync(request, cancellationToken).ConfigureAwait(false);
        }


        /// <summary>
        /// Performs an organisation group search.
        /// </summary>
        /// <param name="client">
        ///   The <see cref="OrganizationInfoClient"/>.
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
        public static async Task<IEnumerable<UserOrGroupPrincipal>> FindGroupsAsync(
            this OrganizationInfoClient client,
            string nameFilter,
            int page = 1,
            int pageSize = 10,
            bool includeExternalResults = false,
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

            return await client.FindGroupsAsync(request, cancellationToken).ConfigureAwait(false);
        }


        /// <summary>
        /// Debits a user for app usage.
        /// </summary>
        /// <param name="client">
        ///   The <see cref="AccountTransactionsClient"/>.
        /// </param>
        /// <param name="amount">
        ///   The number of credits to debit the user. Negative numbers are not allowed.
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
        public static async Task<DebitUserResponse> DebitUserAsync(
            this AccountTransactionsClient client,
            double amount,
            CancellationToken cancellationToken = default
        ) {
            if (client == null) {
                throw new ArgumentNullException(nameof(client));
            }

            var request = new DebitUserRequest() {
                DebitAmount = amount
            };

            return await client.DebitUserAsync(request, cancellationToken).ConfigureAwait(false);
        }


        /// <summary>
        /// Refunds a transaction.
        /// </summary>
        /// <param name="client">
        ///   The <see cref="AccountTransactionsClient"/>.
        /// </param>
        /// <param name="transactionId">
        ///   The ID of the transaction to refund.
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
        public static async Task<RefundUserResponse> RefundUserAsync(
            this AccountTransactionsClient client,
            string transactionId,
            CancellationToken cancellationToken = default
        ) {
            if (client == null) {
                throw new ArgumentNullException(nameof(client));
            }

            var request = new RefundUserRequest() {
                TransactionRef = transactionId
            };

            return await client.RefundUserAsync(request, cancellationToken).ConfigureAwait(false);
        }

    }
}
