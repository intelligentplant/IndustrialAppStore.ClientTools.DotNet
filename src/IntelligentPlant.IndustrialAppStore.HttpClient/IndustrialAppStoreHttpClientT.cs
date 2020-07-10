﻿using System;
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
        /// The client for retrieving information about the authenticated user.
        /// </summary>
        public UserInfoClient<TContext> UserInfo { get; }

        /// <summary>
        /// The client for retrieving information about the authenticated user's organization.
        /// </summary>
        public OrganizationInfoClient<TContext> Organization { get; }

        /// <summary>
        /// The client for performing account transactions.
        /// </summary>
        public AccountTransactionsClient<TContext> AccountTransactions { get; }


        /// <summary>
        /// Creates a new <see cref="IndustrialAppStoreHttpClient{TContext}"/> object.
        /// </summary>
        /// <param name="httpClient">
        ///   The HTTP client to use. When querying the Industrial App Store, an <c>Authorization</c> 
        ///   header must be set on every outgoing request. Use the <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/> 
        ///   method to create a message handler to add the the request pipeline when creating the 
        ///   <paramref name="httpClient"/>, to allow the <see cref="DataCoreHttpClient{TContext}"/> 
        ///   to invoke a callback on demand to retrieve the <c>Authorization</c> header to add to 
        ///   outgoing requests.
        /// </param>
        /// <param name="options">
        ///   The HTTP client options.
        /// </param>
        public IndustrialAppStoreHttpClient(HttpClient httpClient, IndustrialAppStoreHttpClientOptions options) 
            : base(httpClient, options) {

            if (Options.AppStoreUrl == null) {
                throw new ArgumentException(Resources.Error_BaseUrlIsRequired, nameof(options));
            }

            UserInfo = new UserInfoClient<TContext>(HttpClient, Options);
            Organization = new OrganizationInfoClient<TContext>(HttpClient, Options);
            AccountTransactions = new AccountTransactionsClient<TContext>(HttpClient, Options);
        }

    }
}
