using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using IntelligentPlant.DataCore.Client;
using IntelligentPlant.IndustrialAppStore.Client.Model;
using IntelligentPlant.IndustrialAppStore.Client.Queries;

namespace IntelligentPlant.IndustrialAppStore.Client.Clients {
    public class OrganizationInfoClient<TContext> : IasClientBase {

        public OrganizationInfoClient(HttpClient httpClient, IndustrialAppStoreHttpClientOptions options)
            : base(httpClient, options) { }


        public async Task<IEnumerable<UserOrGroupPrincipal>> FindUsersAsync(
            UserOrGroupPrincipalSearchRequest request,
            TContext context = default,
            CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request), true);

            var url = GetAbsoluteUrl("api/user-search/users");

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url) {
                Content = new ObjectContent<UserOrGroupPrincipalSearchRequest>(request, new JsonMediaTypeFormatter())
            }.AddStateProperty(context);

            try {
                using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                    await httpResponse.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await httpResponse.Content.ReadAsAsync<IEnumerable<UserOrGroupPrincipal>>(cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                httpRequest.Dispose();
            }
        }


        public async Task<IEnumerable<UserOrGroupPrincipal>> FindGroupsAsync(
           UserOrGroupPrincipalSearchRequest request,
           TContext context = default,
           CancellationToken cancellationToken = default
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            Validator.ValidateObject(request, new ValidationContext(request), true);

            var url = GetAbsoluteUrl("api/user-search/groups");

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url) { 
                Content = new ObjectContent<UserOrGroupPrincipalSearchRequest>(request, new JsonMediaTypeFormatter())
            }.AddStateProperty(context);

            try {
                using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                    await httpResponse.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await httpResponse.Content.ReadAsAsync<IEnumerable<UserOrGroupPrincipal>>(cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                httpRequest.Dispose();
            }
        }


        public async Task<IEnumerable<UserOrGroupPrincipal>> GetGroupMembershipsAsync(
           TContext context = default,
           CancellationToken cancellationToken = default
        ) {
            var url = GetAbsoluteUrl("api/user-search/me/groups");

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, url).AddStateProperty(context);

            try {
                using (var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false)) {
                    await httpResponse.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await httpResponse.Content.ReadAsAsync<IEnumerable<UserOrGroupPrincipal>>(cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                httpRequest.Dispose();
            }
        }

    }
}
