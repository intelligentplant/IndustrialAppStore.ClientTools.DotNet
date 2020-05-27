using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IntelligentPlant.DataCore.Client;
using IntelligentPlant.IndustrialAppStore.Client.Model;
using IntelligentPlant.IndustrialAppStore.Client.Queries;

namespace IntelligentPlant.IndustrialAppStore.Client.Clients {
    public class AccountTransactionsClient<TContext> : IasClientBase {

        public AccountTransactionsClient(HttpClient httpClient, IndustrialAppStoreHttpClientOptions options)
            : base(httpClient, options) { }


        public async Task<DebitUserResponse> DebitUserAsync(
            DebitUserRequest request,
            TContext context = default,
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request));

            var url = GetAbsoluteUrl("api/resource/debit");

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url) {
                Content = new ObjectContent<double>(request.DebitAmount, new JsonMediaTypeFormatter())
            }.AddStateProperty(context);

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


        public async Task<RefundUserResponse> RefundUserAsync(
            RefundUserRequest request,
            TContext context = default,
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request));

            var url = GetAbsoluteUrl("api/resource/refund");

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url) {
                Content = new ObjectContent<string>(request.TransactionRef, new JsonMediaTypeFormatter())
            }.AddStateProperty(context);

            try {
                using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                    if (httpResponse.IsSuccessStatusCode) {
                        // Success; content body contains the transaction ID.
                        return new RefundUserResponse() {
                            Success = true,
                            Message = Resources.RefundUserResponse_Message_Success
                        };
                    }
                    if (httpResponse.StatusCode == System.Net.HttpStatusCode.BadRequest) {
                        // Transaction failed
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
