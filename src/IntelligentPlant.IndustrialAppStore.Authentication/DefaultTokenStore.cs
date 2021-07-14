using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace IntelligentPlant.IndustrialAppStore.Authentication {

    /// <summary>
    /// Default <see cref="ITokenStore"/> implementation that retrieves tokens from the 
    /// authentication session.
    /// </summary>
    internal class DefaultTokenStore : ITokenStore {

        /// <summary>
        /// The authentication options.
        /// </summary>
        private readonly IndustrialAppStoreAuthenticationOptions _options;

        /// <summary>
        /// The backchannel HTTP client to use.
        /// </summary>
        private readonly HttpClient _backchannelHttpClient;

        /// <summary>
        /// The HTTP context for the current request.
        /// </summary>
        private readonly HttpContext _httpContext;

        /// <summary>
        /// The system clock.
        /// </summary>
        private readonly ISystemClock _clock;


        /// <summary>
        /// Creates a new <see cref="DefaultTokenStore"/> object.
        /// </summary>
        /// <param name="options">
        ///   The authentication options.
        /// </param>
        /// <param name="httpClient">
        ///   The backchannel HTTP client to use.
        /// </param>
        /// <param name="httpContextAccessor">
        ///   The <see cref="IHttpContextAccessor"/> for accessing the <see cref="HttpContext"/> 
        ///   for the current request.
        /// </param>
        /// <param name="clock">
        ///   The system clock.
        /// </param>
        public DefaultTokenStore(
            IndustrialAppStoreAuthenticationOptions options, 
            HttpClient httpClient, 
            IHttpContextAccessor httpContextAccessor,
            ISystemClock clock
        ) {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _backchannelHttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpContext = httpContextAccessor?.HttpContext;
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        }


        /// <inheritdoc/>
        public async Task<string> GetAccessTokenAsync() {
            if (_httpContext == null) {
                return null;
            }

            var authInfo = await _httpContext.AuthenticateAsync();

            return await authInfo.Properties.GetAccessTokenAsync(
                _options, 
                _backchannelHttpClient, 
                _clock, 
                _httpContext.RequestAborted
            );
        }

    }
}
