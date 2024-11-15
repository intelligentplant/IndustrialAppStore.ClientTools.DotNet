using System.Net.Http.Headers;

namespace IntelligentPlant.IndustrialAppStore.DependencyInjection.Internal {

    /// <summary>
    /// <see cref="DelegatingHandler"/> that uses an <see cref="AccessTokenProvider"/> to set an 
    /// access token on outgoing requests.
    /// </summary>
    internal sealed class AccessTokenAuthenticationHandler : DelegatingHandler {

        /// <summary>
        /// The access token provider.
        /// </summary>
        private readonly AccessTokenProvider _accessTokenProvider;


        /// <summary>
        /// Creates a new <see cref="AccessTokenAuthenticationHandler"/> instance.
        /// </summary>
        /// <param name="accessTokenProvider">
        ///   The access token provider.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="accessTokenProvider"/> is <see langword="null"/>.
        /// </exception>
        public AccessTokenAuthenticationHandler(AccessTokenProvider accessTokenProvider) {
            _accessTokenProvider = accessTokenProvider ?? throw new ArgumentNullException(nameof(accessTokenProvider));
        }


        /// <inheritdoc/>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            var accessToken = _accessTokenProvider.Factory == null
                ? null
                : await _accessTokenProvider.Factory.Invoke(cancellationToken).ConfigureAwait(false);

            if (!string.IsNullOrWhiteSpace(accessToken)) {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }

    }
}
