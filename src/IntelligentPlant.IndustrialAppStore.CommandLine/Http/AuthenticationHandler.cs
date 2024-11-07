using System.Net.Http;
using System.Net.Http.Headers;

namespace IntelligentPlant.IndustrialAppStore.CommandLine.Http {

    /// <summary>
    /// A <see cref="DelegatingHandler"/> that obtains an access token from an <see cref="IndustrialAppStoreSessionManager"/> 
    /// and attaches it to outgoing requests.
    /// </summary>
    internal sealed class AuthenticationHandler : DelegatingHandler {

        /// <summary>
        /// The session manager.
        /// </summary>
        private readonly IndustrialAppStoreSessionManager _sessionManager;


        /// <summary>
        /// Creates a new <see cref="AuthenticationHandler"/> instance.
        /// </summary>
        /// <param name="sessionManager">
        ///   The session manager.
        /// </param>
        public AuthenticationHandler(IndustrialAppStoreSessionManager sessionManager) {
            _sessionManager = sessionManager;
        }


        /// <inheritdoc/>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            var accessToken = await _sessionManager.GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);
            if (accessToken != null) {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }

    }
}
