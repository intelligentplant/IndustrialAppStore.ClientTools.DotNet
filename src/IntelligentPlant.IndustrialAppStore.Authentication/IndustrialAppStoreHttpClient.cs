using System;
using System.Net.Http;
using System.Threading.Tasks;
using IntelligentPlant.IndustrialAppStore.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace IntelligentPlant.IndustrialAppStore.Authentication {

    /// <summary>
    /// HTTP client for performing Industrial App Store API requests on behalf of an authenticated 
    /// user.
    /// </summary>
    public class IndustrialAppStoreHttpClient : IndustrialAppStoreHttpClient<HttpContext> {

        /// <summary>
        /// Creates a new <see cref="IndustrialAppStoreHttpClient"/> object.
        /// </summary>
        /// <param name="httpClient">
        ///   The HTTP client to use. When querying the Industrial App Store, an <c>Authorization</c> 
        ///   header must be set on every outgoing request. Use the <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/> 
        ///   method to create a message handler to add the the request pipeline when creating the 
        ///   <paramref name="httpClient"/>, to allow the <see cref="IndustrialAppStoreHttpClient"/> 
        ///   to invoke a callback on demand to retrieve the <c>Authorization</c> header to add to 
        ///   outgoing requests.
        /// </param>
        public IndustrialAppStoreHttpClient(HttpClient httpClient, IndustrialAppStoreHttpClientOptions options) 
            : base(httpClient, options) { }


        /// <summary>
        /// Tests if the specified HTTP context has a valid access token associated with it.
        /// </summary>
        /// <param name="context">
        ///   The HTTP context.
        /// </param>
        /// <returns>
        ///   A <see cref="Task{TResult}"/> that will return a flag indiciating if a valid access 
        ///   token is available.
        /// </returns>
        public async Task<bool> HasValidAccessToken(HttpContext context) {
            if (context == null) {
                throw new ArgumentNullException(nameof(context));
            }

            var token = await context
                .RequestServices
                .GetRequiredService<ITokenStore>()
                .GetAccessTokenAsync()
                .ConfigureAwait(false);

            return !string.IsNullOrWhiteSpace(token);
        }

    }
}
