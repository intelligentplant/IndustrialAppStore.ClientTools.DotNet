using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IntelligentPlant.IndustrialAppStore.Authentication.Http {

    /// <summary>
    /// <see cref="DelegatingHandler"/> that authenticates outgoing requests using an Industrial 
    /// App Store access token obtained from an <see cref="ITokenStore"/>.
    /// </summary>
    internal sealed class TokenStoreAuthenticationHandler : DelegatingHandler {

        /// <summary>
        /// The token store.
        /// </summary>
        private readonly ITokenStore _tokenStore;


        /// <summary>
        /// Creates a new <see cref="TokenStoreAuthenticationHandler"/> instance.
        /// </summary>
        /// <param name="tokenStore">
        ///   The <see cref="ITokenStore"/> to use.
        /// </param>
        public TokenStoreAuthenticationHandler(ITokenStore tokenStore) {
            ArgumentNullException.ThrowIfNull(tokenStore);
            _tokenStore = tokenStore;
        }


        /// <inheritdoc/>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            var authHeader = await _tokenStore.GetAuthenticationHeaderAsync().ConfigureAwait(false);
            if (authHeader != null) {
                request.Headers.Authorization = authHeader;
            }

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }

    }
}
