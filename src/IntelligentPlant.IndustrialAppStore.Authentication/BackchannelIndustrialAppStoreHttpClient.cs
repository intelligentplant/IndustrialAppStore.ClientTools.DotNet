using System;
using System.Net.Http;
using System.Threading.Tasks;

using IntelligentPlant.DataCore.Client;
using IntelligentPlant.IndustrialAppStore.Client;

using Microsoft.Extensions.Hosting;

namespace IntelligentPlant.IndustrialAppStore.Authentication {

    /// <summary>
    /// HTTP client for performing Industrial App Store API requests on behalf of an authenticated 
    /// user outside of the context of an HTTP request, for example in an <see cref="IHostedService"/> 
    /// or a background task.
    /// </summary>
    public sealed class BackchannelIndustrialAppStoreHttpClient : IndustrialAppStoreHttpClient<ITokenStore> {

        /// <inheritdoc/>
        public override bool AllowIasApiOperations { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="BackchannelIndustrialAppStoreHttpClient"/> class.
        /// </summary>
        /// <param name="httpClient">
        ///   The HTTP client to use. When querying the Industrial App Store, an <c>Authorization</c> 
        ///   header must be set on every outgoing request. Use the <see cref="CreateAuthenticationMessageHandler"/> 
        ///   method to create a message handler to add the the request pipeline when creating the 
        ///   <paramref name="httpClient"/>, to allow the <see cref="BackchannelIndustrialAppStoreHttpClient"/> 
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
        public BackchannelIndustrialAppStoreHttpClient(
            HttpClient httpClient, 
            IndustrialAppStoreHttpClientOptions options, 
            IndustrialAppStoreAuthenticationOptions authenticationOptions
        ) : base(httpClient, options) { 
            AllowIasApiOperations = !authenticationOptions.UseExternalAuthentication;
        }


        /// <summary>
        /// Creates a <see cref="DelegatingHandler"/> that can be added to an <see cref="HttpClient"/> 
        /// message pipeline, that will set the <c>Authorize</c> header on outgoing requests based 
        /// on the <see cref="ITokenStore"/> passed to an <see cref="BackchannelIndustrialAppStoreHttpClient"/> 
        /// method.
        /// </summary>
        /// <param name="callback">
        ///   The callback delegate that will receive the outgoing request and the <see cref="ITokenStore"/> 
        ///   specified in the invocation and return the <c>Authorize</c> header value to add to 
        ///   the outgoing request.
        /// </param>
        /// <returns>
        ///   A new <see cref="DelegatingHandler"/> object.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="callback"/> is <see langword="null"/>.
        /// </exception>
        public static new DelegatingHandler CreateAuthenticationMessageHandler(AuthenticationCallback<ITokenStore> callback) {
            return DataCoreHttpClient.CreateAuthenticationMessageHandler(callback);
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
            return await IndustrialAppStoreHttpClient.HasValidAccessToken(tokenStore ?? throw new ArgumentNullException(nameof(tokenStore))).ConfigureAwait(false);
        }

    }
}
