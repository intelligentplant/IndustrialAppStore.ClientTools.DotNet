using System;
using System.Net.Http;
using IntelligentPlant.DataCore.Client;
using IntelligentPlant.IndustrialAppStore.Client.Clients;

namespace IntelligentPlant.IndustrialAppStore.Client {

    /// <summary>
    /// HTTP client for performing Industrial App Store API requests on behalf of an authenticated 
    /// user.
    /// </summary>
    /// <typeparam name="TContext">
    ///   The context type that is passed to API calls to allow authentication headers to be added 
    ///   to outgoing requests.
    /// </typeparam>
    public class IndustrialAppStoreHttpClient<TContext> : DataCoreHttpClient<TContext, IndustrialAppStoreHttpClientOptions> {

        /// <summary>
        /// Specifies if Industrial App Store-specific API operations are allowed. When <see langword="false"/>, 
        /// attempts to access the <see cref="UserInfo"/>, <see cref="Organization"/> and <see cref="AccountTransactions"/> 
        /// properties will cause an <see cref="InvalidOperationException"/> to be thrown.
        /// </summary>
        public virtual bool AllowIasApiOperations => true;

        /// <summary>
        /// The client for retrieving information about the authenticated user.
        /// </summary>
        private readonly UserInfoClient<TContext> _userInfo;

        /// <summary>
        /// The client for retrieving information about the authenticated user's organization.
        /// </summary>
        private readonly OrganizationInfoClient<TContext> _organization;

        /// <summary>
        /// The client for performing account transactions.
        /// </summary>
        private readonly AccountTransactionsClient<TContext> _accountTransactions;

        /// <summary>
        /// The client for retrieving information about the authenticated user.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///   <see cref="AllowIasApiOperations"/> is <see langword="false"/>.
        /// </exception>
        public UserInfoClient<TContext> UserInfo => AllowIasApiOperations ? _userInfo : throw new InvalidOperationException(Resources.Error_IasOperationsAreNotAllowed);

        /// <summary>
        /// The client for retrieving information about the authenticated user's organization.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///   <see cref="AllowIasApiOperations"/> is <see langword="false"/>.
        /// </exception>
        public OrganizationInfoClient<TContext> Organization => AllowIasApiOperations ? _organization : throw new InvalidOperationException(Resources.Error_IasOperationsAreNotAllowed);

        /// <summary>
        /// The client for performing account transactions.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///   <see cref="AllowIasApiOperations"/> is <see langword="false"/>.
        /// </exception>
        public AccountTransactionsClient<TContext> AccountTransactions => AllowIasApiOperations ? _accountTransactions : throw new InvalidOperationException(Resources.Error_IasOperationsAreNotAllowed);


        /// <summary>
        /// Creates a new <see cref="IndustrialAppStoreHttpClient{TContext}"/> object.
        /// </summary>
        /// <param name="httpClient">
        ///   The HTTP client to use. When querying the Industrial App Store, an <c>Authorization</c> 
        ///   header must be set on every outgoing request. Use the <see cref="CreateAuthenticationMessageHandler"/> 
        ///   method to create a message handler to add the the request pipeline when creating the 
        ///   <paramref name="httpClient"/>, to allow the <see cref="IndustrialAppStoreHttpClient{TContext}"/> 
        ///   to invoke a callback on demand to retrieve the <c>Authorization</c> header to add to 
        ///   outgoing requests.
        /// </param>
        /// <param name="options">
        ///   The HTTP client options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="httpClient"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="options"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   <see cref="DataCoreHttpClientOptions.DataCoreUrl"/> on <paramref name="options"/> is 
        ///   <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   <see cref="IndustrialAppStoreHttpClientOptions.IndustrialAppStoreUrl"/> on 
        ///   <paramref name="options"/> is <see langword="null"/>.
        /// </exception>
        public IndustrialAppStoreHttpClient(HttpClient httpClient, IndustrialAppStoreHttpClientOptions options) 
            : base(httpClient, options) {

            if (Options.IndustrialAppStoreUrl == null) {
                throw new ArgumentException(Resources.Error_BaseUrlIsRequired, nameof(options));
            }

            _userInfo = new UserInfoClient<TContext>(HttpClient, Options);
            _organization = new OrganizationInfoClient<TContext>(HttpClient, Options);
            _accountTransactions = new AccountTransactionsClient<TContext>(HttpClient, Options);
        }


        /// <summary>
        /// Creates a <see cref="DelegatingHandler"/> that can be added to an <see cref="HttpClient"/> 
        /// message pipeline, that will set the <c>Authorize</c> header on outgoing requests based 
        /// on the <typeparamref name="TContext"/> object passed to an <see cref="IndustrialAppStoreHttpClient{TContext}"/> 
        /// method.
        /// </summary>
        /// <param name="callback">
        ///   The callback delegate that will receive the HTTP request and a <typeparamref name="TContext"/> 
        ///   objectand return the <c>Authorize</c> header value to add to the request.
        /// </param>
        /// <returns>
        ///   A new <see cref="DelegatingHandler"/> object.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="callback"/> is <see langword="null"/>.
        /// </exception>
#pragma warning disable CA1000 // Do not declare static members on generic types
        public static DelegatingHandler CreateAuthenticationMessageHandler(AuthenticationCallback<TContext> callback) {
#pragma warning restore CA1000 // Do not declare static members on generic types
            return DataCoreHttpClient.CreateAuthenticationMessageHandler(callback);
        }

    }
}
