using IntelligentPlant.DataCore.Client;
using IntelligentPlant.IndustrialAppStore.Client.Clients;

namespace IntelligentPlant.IndustrialAppStore.Client {

    /// <summary>
    /// HTTP client for performing Industrial App Store API requests on behalf of an authenticated 
    /// user.
    /// </summary>
    public class IndustrialAppStoreHttpClient : DataCoreHttpClient {

        /// <summary>
        /// Specifies if Industrial App Store-specific API operations are allowed. When <see langword="false"/>, 
        /// attempts to access the <see cref="UserInfo"/>, <see cref="Organization"/> and <see cref="AccountTransactions"/> 
        /// properties will cause an <see cref="InvalidOperationException"/> to be thrown.
        /// </summary>
        public virtual bool AllowIasApiOperations => true;

        /// <summary>
        /// The client for retrieving information about the authenticated user.
        /// </summary>
        private readonly UserInfoClient _userInfo;

        /// <summary>
        /// The client for retrieving information about the authenticated user's organization.
        /// </summary>
        private readonly OrganizationInfoClient _organization;

        /// <summary>
        /// The client for performing account transactions.
        /// </summary>
        private readonly AccountTransactionsClient _accountTransactions;

        /// <summary>
        /// The client for retrieving information about the authenticated user.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///   <see cref="AllowIasApiOperations"/> is <see langword="false"/>.
        /// </exception>
        public UserInfoClient UserInfo => AssertIasOperationAllowed(_userInfo);

        /// <summary>
        /// The client for retrieving information about the authenticated user's organization.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///   <see cref="AllowIasApiOperations"/> is <see langword="false"/>.
        /// </exception>
        public OrganizationInfoClient Organization => AssertIasOperationAllowed(_organization);

        /// <summary>
        /// The client for performing account transactions.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///   <see cref="AllowIasApiOperations"/> is <see langword="false"/>.
        /// </exception>
        public AccountTransactionsClient AccountTransactions => AssertIasOperationAllowed(_accountTransactions);


        /// <summary>
        /// Creates a new <see cref="IndustrialAppStoreHttpClient"/> object.
        /// </summary>
        /// <param name="httpClient">
        ///   The HTTP client to use.
        /// </param>
        /// <param name="options">
        ///   The client options.
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
        public IndustrialAppStoreHttpClient(HttpClient httpClient, IndustrialAppStoreHttpClientOptions options) : base(httpClient, options) {
            if (options.IndustrialAppStoreUrl == null) {
                throw new ArgumentException(Resources.Error_BaseUrlIsRequired, nameof(options));
            }

            _userInfo = new UserInfoClient(HttpClient, options);
            _organization = new OrganizationInfoClient(HttpClient, options);
            _accountTransactions = new AccountTransactionsClient(HttpClient, options);
        }


        /// <summary>
        /// Throws an <see cref="InvalidOperationException"/> if <see cref="AllowIasApiOperations"/> 
        /// is <see langword="false"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///   <see cref="AllowIasApiOperations"/> is <see langword="false"/>.
        /// </exception>
        protected void AssertIasOperationAllowed() {
            if (!AllowIasApiOperations) {
                throw new InvalidOperationException(Resources.Error_IasOperationsAreNotAllowed);
            }
        }


        /// <summary>
        /// Returns the specified <paramref name="value"/> if <see cref="AllowIasApiOperations"/> 
        /// is <see langword="true"/>, or throws an <see cref="InvalidOperationException"/> if 
        /// <see cref="AllowIasApiOperations"/> is <see langword="false"/>.
        /// </summary>
        /// <typeparam name="T">
        ///   The value type.
        /// </typeparam>
        /// <param name="value">
        ///   The value to return.
        /// </param>
        /// <returns>
        ///   The <paramref name="value"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///   <see cref="AllowIasApiOperations"/> is <see langword="false"/>.
        /// </exception>
        /// <remarks>
        ///   Call this method to guard against attempts to perform Industrial App Store-specific 
        ///   operations when an app is running in on-premises mode.
        /// </remarks>
        protected T AssertIasOperationAllowed<T>(T value) {
            AssertIasOperationAllowed();

            return value;
        }

    }
}
