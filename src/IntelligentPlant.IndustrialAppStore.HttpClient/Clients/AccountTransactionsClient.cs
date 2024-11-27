using System.ComponentModel.DataAnnotations;
using System.Globalization;

using IntelligentPlant.DataCore.Client;
using IntelligentPlant.IndustrialAppStore.Client.Model;
using IntelligentPlant.IndustrialAppStore.Client.Queries;

namespace IntelligentPlant.IndustrialAppStore.Client.Clients {

    /// <summary>
    /// Client for performing account transactions.
    /// </summary>
    public class AccountTransactionsClient : IasClientBase {

        /// <summary>
        /// Creates a new <see cref="AccountTransactionsClient"/> object.
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
        /// <exception cref="ArgumentException">
        ///   <paramref name="options"/> is <see langword="null"/>.
        /// </exception>
        internal AccountTransactionsClient(HttpClient httpClient, IndustrialAppStoreHttpClientOptions options)
            : base(httpClient, options) { }


        /// <summary>
        /// Debits a user for app usage.
        /// </summary>
        /// <param name="request">
        ///   The request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A <see cref="Task{TResult}"/> that returns the debit response.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="request"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ValidationException">
        ///   <paramref name="request"/> fails validation.
        /// </exception>
        public async Task<DebitUserResponse> DebitUserAsync(
            DebitUserRequest request,
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request));

            var url = GetAbsoluteUrl("api/resource/debit");

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url) {
                Content = CreateJsonContent(request.DebitAmount)
            };

            try {
                using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                    if (httpResponse.StatusCode == System.Net.HttpStatusCode.OK) {
                        // Success; content body contains the transaction ID.
                        return new DebitUserResponse() { 
                            Success = true,
                            TransactionId = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false),
                            Message = Resources.DebitUserResponse_Message_Success
                        };
                    }
                    if (httpResponse.StatusCode == System.Net.HttpStatusCode.NotModified) {
                        // Transaction failed
                        return new DebitUserResponse() {
                            Success = false,
                            TransactionId = null,
                            Message = Resources.DebitUserResponse_Message_TransactionFailed
                        };
                    }
                    if (httpResponse.StatusCode == System.Net.HttpStatusCode.BadRequest) {
                        // Insufficient funds
                        return new DebitUserResponse() {
                            Success = false,
                            TransactionId = null,
                            Message = Resources.DebitUserResponse_Message_InsufficientFunds
                        };
                    }

                    await httpResponse.ThrowOnErrorResponse().ConfigureAwait(false);

                    // Any other status is unexpected.
                    return new DebitUserResponse() {
                        Success = false,
                        TransactionId = null,
                        Message = string.Format(
                            CultureInfo.CurrentCulture, 
                            Resources.DebitUserResponse_Message_UnexpectedResponseCode, 
                            (int) httpResponse.StatusCode
                        )
                    };
                }
            }
            finally {
                httpRequest.Dispose();
            }
        }


        /// <summary>
        /// Refunds a transaction.
        /// </summary>
        /// <param name="request">
        ///   The refund request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A <see cref="Task{TResult}"/> that will return a <see cref="RefundUserResponse"/> 
        ///   specifying if the operation was successful.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="request"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ValidationException">
        ///   <paramref name="request"/> fails validation.
        /// </exception>
        public async Task<RefundUserResponse> RefundUserAsync(
            RefundUserRequest request,
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request));

            var url = GetAbsoluteUrl("api/resource/refund");

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url) {
                Content = CreateJsonContent(request.TransactionRef)
            };

            try {
                using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                    if (httpResponse.IsSuccessStatusCode) {
                        // Success.
                        return new RefundUserResponse() {
                            Success = true,
                            Message = Resources.RefundUserResponse_Message_Success
                        };
                    }
                    if (httpResponse.StatusCode == System.Net.HttpStatusCode.BadRequest) {
                        // Refund failed
                        return new RefundUserResponse() {
                            Success = false,
                            Message = string.Format(
                                CultureInfo.CurrentCulture, 
                                Resources.RefundUserResponse_Message_Failed, 
                                await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false)
                            )
                        };
                    }

                    await httpResponse.ThrowOnErrorResponse().ConfigureAwait(false);

                    // Any other status is unexpected.
                    return new RefundUserResponse() {
                        Success = false,
                        Message = string.Format(
                            CultureInfo.CurrentCulture, 
                            Resources.RefundUserResponse_Message_UnexpectedResponseCode, 
                            (int) httpResponse.StatusCode
                        )
                    };
                }
            }
            finally {
                httpRequest.Dispose();
            }
        }

    }
}
