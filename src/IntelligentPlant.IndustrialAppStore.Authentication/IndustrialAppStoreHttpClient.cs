using System;
using System.Net.Http;
using System.Threading.Tasks;

using IntelligentPlant.DataCore.Client;
using IntelligentPlant.IndustrialAppStore.Client;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace IntelligentPlant.IndustrialAppStore.Authentication {

    /// <summary>
    /// HTTP client for performing Industrial App Store API requests on behalf of an authenticated 
    /// user during an HTTP request to the application.
    /// </summary>
    public class IndustrialAppStoreHttpClient : IndustrialAppStoreHttpClient<HttpContext> {

        /// <inheritdoc/>
        public override bool AllowIasApiOperations { get; }

        /// <summary>
        /// Creates a new <see cref="IndustrialAppStoreHttpClient"/> object.
        /// </summary>
        /// <param name="httpClient">
        ///   The HTTP client to use. When querying the Industrial App Store, an <c>Authorization</c> 
        ///   header must be set on every outgoing request. Use the <see cref="CreateAuthenticationMessageHandler"/> 
        ///   method to create a message handler to add the the request pipeline when creating the 
        ///   <paramref name="httpClient"/>, to allow the <see cref="IndustrialAppStoreHttpClient"/> 
        ///   to invoke a callback on demand to retrieve the <c>Authorization</c> header to add to 
        ///   outgoing requests.
        /// </param>
        /// <param name="options">
        ///   The HTTP client options.
        /// </param>
        /// <param name="authenticationOptions">
        ///   The authentication options for the host app. When the <see cref="IndustrialAppStoreAuthenticationOptions.UseExternalAuthentication"/> 
        ///   property is <see langword="true"/>, the <see cref="AllowIasApiOperations"/> flag will be 
        ///   set to <see langword="false"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="httpClient"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="options"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="authenticationOptions"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   <see cref="DataCoreHttpClientOptions.DataCoreUrl"/> on <paramref name="options"/> is 
        ///   <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   <see cref="IndustrialAppStoreHttpClientOptions.IndustrialAppStoreUrl"/> on 
        ///   <paramref name="options"/> is <see langword="null"/>.
        /// </exception>
        public IndustrialAppStoreHttpClient(
            HttpClient httpClient, 
            IndustrialAppStoreHttpClientOptions options, 
            IndustrialAppStoreAuthenticationOptions authenticationOptions
        ) : base(httpClient, options) { 
            if (authenticationOptions == null) {
                throw new ArgumentNullException(nameof(authenticationOptions));
            }

            AllowIasApiOperations = !authenticationOptions.UseExternalAuthentication;
        }


        /// <summary>
        /// Creates a <see cref="DelegatingHandler"/> that can be added to an <see cref="HttpClient"/> 
        /// message pipeline, that will set the <c>Authorize</c> header on outgoing requests based 
        /// on the <see cref="HttpContext"/> passed to an <see cref="IndustrialAppStoreHttpClient"/> 
        /// method.
        /// </summary>
        /// <param name="callback">
        ///   The callback delegate that will receive the outgoing request and the <see cref="HttpContext"/> 
        ///   specified in the invocation and return the <c>Authorize</c> header value to add to 
        ///   the outgoing request.
        /// </param>
        /// <returns>
        ///   A new <see cref="DelegatingHandler"/> object.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="callback"/> is <see langword="null"/>.
        /// </exception>
        public static new DelegatingHandler CreateAuthenticationMessageHandler(AuthenticationCallback<HttpContext> callback) {
            return DataCoreHttpClient.CreateAuthenticationMessageHandler(callback);
        }


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
        public static async Task<bool> HasValidAccessToken(HttpContext context) {
            if (context == null) {
                throw new ArgumentNullException(nameof(context));
            }

            return await HasValidAccessToken(context.RequestServices.GetRequiredService<ITokenStore>()).ConfigureAwait(false);
        }


        /// <summary>
        /// Tests if the specified <see cref="ITokenStore"/> has a valid access token associated with it.
        /// </summary>
        /// <param name="tokenStore">
        ///   The token store.
        /// </param>
        /// <returns>
        ///   A <see cref="Task{TResult}"/> that will return a flag indiciating if a valid access 
        ///   token is available.
        /// </returns>
        public static async Task<bool> HasValidAccessToken(ITokenStore tokenStore) {
            if (tokenStore == null) {
                throw new ArgumentNullException(nameof(tokenStore));
            }

            var token = await tokenStore
                .GetTokensAsync()
                .ConfigureAwait(false);

            return !string.IsNullOrWhiteSpace(token?.AccessToken);
        }

    }
}
