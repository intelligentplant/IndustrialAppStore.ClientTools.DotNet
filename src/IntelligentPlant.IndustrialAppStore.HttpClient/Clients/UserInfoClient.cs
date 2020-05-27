using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IntelligentPlant.DataCore.Client;
using IntelligentPlant.IndustrialAppStore.Client.Model;

namespace IntelligentPlant.IndustrialAppStore.Client.Clients {
    public class UserInfoClient<TContext> : IasClientBase {

        public UserInfoClient(HttpClient httpClient, IndustrialAppStoreHttpClientOptions options)
            : base(httpClient, options) { }


        public async Task<UserOrGroupPrincipal> GetUserInfoAsync(
            TContext context = default,
            CancellationToken cancellationToken = default
        ) {
            var url = GetAbsoluteUrl("api/user-search/me");

            var request = new HttpRequestMessage(HttpMethod.Get, url).AddStateProperty(context);

            try {
                using (var response = await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false)) {
                    await response.ThrowOnErrorResponse().ConfigureAwait(false);
                    return await response.Content.ReadAsAsync<UserOrGroupPrincipal>(cancellationToken).ConfigureAwait(false);
                }
            }
            finally {
                request.Dispose();
            }
        }

    }
}
