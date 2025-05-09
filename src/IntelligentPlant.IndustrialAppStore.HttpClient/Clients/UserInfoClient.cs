using IntelligentPlant.DataCore.Client;
using IntelligentPlant.IndustrialAppStore.Client.Model;

namespace IntelligentPlant.IndustrialAppStore.Client.Clients {

    /// <summary>
    /// Client for querying an Industrial App Store user information.
    /// </summary>
    public class UserInfoClient : IasClientBase {

        /// <summary>
        /// Creates a new <see cref="UserInfoClient"/> object.
        /// </summary>
        /// <param name="httpClient">
        ///   The HTTP client to use.
        /// </param>
        /// <param name="options">
        ///   The HTTP client options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="httpClient"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   <paramref name="options"/> is <see langword="null"/>.
        /// </exception>
        internal UserInfoClient(HttpClient httpClient, IndustrialAppStoreHttpClientOptions options)
            : base(httpClient, options) { }


        /// <summary>
        /// Gets information about the calling user.
        /// </summary>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   A task that will return the user information.
        /// </returns>
        public async Task<UserOrGroupPrincipal> GetUserInfoAsync(
            CancellationToken cancellationToken = default
        ) {
            var url = GetAbsoluteUrl("api/user-search/me");

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            try {
                using (var response = await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false)) {
                    await response.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await ReadFromJsonAsync<UserOrGroupPrincipal>(response, cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                request.Dispose();
            }
        }

    }
}
