using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;

using DataCore.Adapter.Extensions;

namespace IntelligentPlant.DataCore.Client.Clients {

    partial class AdaptersClient {

        /// <summary>
        /// Client for App Store Connect adapter custom functions.
        /// </summary>
        public class CustomFunctionsClient {

            private readonly AdaptersClient _client;


            internal CustomFunctionsClient(AdaptersClient client) {
                _client = client;
            }


            /// <summary>
            /// Gets the custom functions exposed by the specified driver.
            /// </summary>
            /// <param name="adapterId">
            ///   The adapter ID.
            /// </param>
            /// <param name="cancellationToken">
            ///   The cancellation token for the operation.
            /// </param>
            /// <returns>
            ///   The custom functions exposed by the specified adapter.
            /// </returns>
            /// <exception cref="ArgumentException">
            ///   <paramref name="adapterId"/> is <see langword="null"/> or white space.
            /// </exception>
            public async Task<IEnumerable<CustomFunctionDescriptor>> GetCustomFunctionsAsync(string adapterId, CancellationToken cancellationToken) {
                if (string.IsNullOrWhiteSpace(adapterId)) {
                    throw new ArgumentException(Resources.Error_DriverIdIsRequired, nameof(adapterId));
                }

                var url = _client.GetAbsoluteUrl($"api/rpc/v2/{Uri.EscapeDataString(adapterId)}/functions");

                using var response = await _client.HttpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
                await response.ThrowOnErrorResponse().ConfigureAwait(false);

                return await response.Content.ReadFromJsonAsync<IEnumerable<CustomFunctionDescriptor>>(_client.Options.JsonOptions, cancellationToken).ConfigureAwait(false) ?? Array.Empty<CustomFunctionDescriptor>();
            }


            /// <summary>
            /// Gets detailed information about a custom function.
            /// </summary>
            /// <param name="adapterId">
            ///   The adapter ID.
            /// </param>
            /// <param name="functionId">
            ///   The custom function ID.
            /// </param>
            /// <param name="cancellationToken">
            ///   The cancellation token for the operation.
            /// </param>
            /// <returns>
            ///   The custom function details, or <see langword="null"/> if the function could not be found.
            /// </returns>
            /// <exception cref="ArgumentException">
            ///   <paramref name="adapterId"/> is <see langword="null"/> or white space.
            /// </exception>
            /// <exception cref="ArgumentNullException">
            ///   <paramref name="functionId"/> is <see langword="null"/>.
            /// </exception>
            public async Task<CustomFunctionDescriptorExtended?> GetCustomFunctionAsync(string adapterId, Uri functionId, CancellationToken cancellationToken) {
                if (string.IsNullOrWhiteSpace(adapterId)) {
                    throw new ArgumentException(Resources.Error_DriverIdIsRequired, nameof(adapterId));
                }
                if (functionId == null) {
                    throw new ArgumentNullException(nameof(functionId));
                }

                var url = _client.GetAbsoluteUrl($"api/rpc/v2/{Uri.EscapeDataString(adapterId)}/functions/details?id={Uri.EscapeDataString(functionId.ToString())}");
                using var response = await _client.HttpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
                await response.ThrowOnErrorResponse().ConfigureAwait(false);
                return await response.Content.ReadFromJsonAsync<CustomFunctionDescriptorExtended>(_client.Options.JsonOptions, cancellationToken).ConfigureAwait(false);
            }


            /// <summary>
            /// Invokes a custom function.
            /// </summary>
            /// <param name="adapterId">
            ///   The adapter ID.
            /// </param>
            /// <param name="request">
            ///   The custom function invocation request.
            /// </param>
            /// <param name="cancellationToken">
            ///   The cancellation token for the operation.
            /// </param>
            /// <returns>
            ///   The custom function invocation response.
            /// </returns>
            /// <exception cref="ArgumentException">
            ///   <paramref name="adapterId"/> is <see langword="null"/> or white space.
            /// </exception>
            /// <exception cref="ArgumentNullException">
            ///   <paramref name="request"/> is <see langword="null"/>.
            /// </exception>
            /// <exception cref="ValidationException">
            ///   <paramref name="request"/> fails validation.
            /// </exception>
            public async Task<CustomFunctionInvocationResponse> InvokeCustomFunctionAsync(string adapterId, CustomFunctionInvocationRequest request, CancellationToken cancellationToken) {
                if (string.IsNullOrWhiteSpace(adapterId)) {
                    throw new ArgumentException(Resources.Error_DriverIdIsRequired, nameof(adapterId));
                }
                if (request == null) {
                    throw new ArgumentNullException(nameof(request));
                }

                Validator.ValidateObject(request, new ValidationContext(request), true);

                var url = _client.GetAbsoluteUrl($"api/rpc/v2/{Uri.EscapeDataString(adapterId)}/functions/invoke");
                using var response = await _client.HttpClient.PostAsJsonAsync(url, request, _client.Options.JsonOptions, cancellationToken).ConfigureAwait(false);
                await response.ThrowOnErrorResponse().ConfigureAwait(false);
                var result = await response.Content.ReadFromJsonAsync<CustomFunctionInvocationResponse>(_client.Options.JsonOptions, cancellationToken).ConfigureAwait(false);
                return result!;
            }

        }

    }
}
