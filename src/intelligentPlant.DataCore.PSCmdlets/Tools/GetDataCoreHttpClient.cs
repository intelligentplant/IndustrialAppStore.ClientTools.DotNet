using IntelligentPlant.DataCore.Client;

namespace IntelligentPlant.DataCore.PSCmdlets.Tools
{
    public class GetDataCoreHttpClient
    {
        /// <summary>
        /// Return an instance of a Data Core Http Client
        /// </summary>

        private static GetDataCoreHttpClient _instance;

        private GetDataCoreHttpClient() { }

        public static GetDataCoreHttpClient Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GetDataCoreHttpClient();
                }
                return _instance;
            }
        }

        public DataCoreHttpClient CreateDataCoreClient(HttpClient httpClient, string dataCoreUrl)
        {
            var _dataCoreClient = new DataCoreHttpClient(httpClient, new DataCoreHttpClientOptions
            {
                DataCoreUrl = new Uri(dataCoreUrl)
            });

            return _dataCoreClient;
        }
    }
}
