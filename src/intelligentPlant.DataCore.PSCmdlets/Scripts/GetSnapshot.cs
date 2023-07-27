using IntelligentPlant.DataCore.PSCmdlets.Tools;
using IntelligentPlant.DataCore.Client;
using IntelligentPlant.DataCore.Client.Queries;

namespace IntelligentPlant.DataCore.PSCmdlets.Scripts
{
    public class GetSnapshot
    {
        private static DataCoreHttpClient DataCoreHttpClient;


        public void Init(string dataCoreUrl)
        {
            var httpClient = GetHttpClient.Instance.CreateHttpClient(dataCoreUrl);

            DataCoreHttpClient = new DataCoreHttpClient(httpClient, new DataCoreHttpClientOptions()
            {
                DataCoreUrl = new Uri(dataCoreUrl)
            });
        }

        public double ReadSnapshotNumeric(string dataSourceName, string tagName)
        {
            var tags = new Dictionary<string, string[]>
            {
                { dataSourceName, new string[] { tagName } }
            };

            var request = new ReadSnapshotTagValuesRequest
            {
                Tags = tags
            };
            var result = DataCoreHttpClient.DataSources.ReadSnapshotTagValuesAsync(request).Result;
            return result[dataSourceName].Values.FirstOrDefault().NumericValue;
        }
    }
}