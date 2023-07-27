using System.Net;
using System.Net.Http.Headers;
using IntelligentPlant.DataCore.Client;

namespace IntelligentPlant.DataCore.PSCmdlets.Tools
{
    public class GetHttpClient
    {
        /// <summary>
        /// Return an instance of a HTTP client
        /// </summary>


        const string AccessToken = "";
        private static readonly ICredentials DefaultCredentials = CredentialCache.DefaultNetworkCredentials;
        private string _url;

        private static GetHttpClient _instance;

        private GetHttpClient() { }

        public static GetHttpClient Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GetHttpClient();
                }
                return _instance;
            }
        }

        public HttpClient CreateHttpClient(string dataCoreUrl)
        {
            var handler = new HttpClientHandler();

            if (string.IsNullOrWhiteSpace(AccessToken))
            {
                handler.Credentials = DefaultCredentials;
            }

            var client = HttpClientFactory.Create(handler, DataCoreHttpClient.CreateAuthenticationMessageHandler<object>(async (request, state, ct) =>
            {
                return string.IsNullOrWhiteSpace(AccessToken)
                    ? null
                    : new AuthenticationHeaderValue("Bearer", AccessToken);
            }));

            client.BaseAddress = new Uri(dataCoreUrl);
            return client;
        }
    }
}