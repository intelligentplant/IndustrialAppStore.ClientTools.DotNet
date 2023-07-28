using IntelligentPlant.DataCore.PSCmdlets.Tools;
using IntelligentPlant.DataCore.Client;
using IntelligentPlant.DataCore.Client.Queries;
using System.Net.Http;

namespace IntelligentPlant.DataCore.PSCmdlets.Scripts
{
    public class GetSnapshot
    {
        private static DataCoreHttpClient dataCoreClient;


        public void Init(string dataCoreUrl)
        {
            var httpClient = GetHttpClient.Instance.CreateHttpClient(dataCoreUrl);

            dataCoreClient = GetDataCoreHttpClient.Instance.CreateDataCoreClient(httpClient, dataCoreUrl);
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
            var result = dataCoreClient.DataSources.ReadSnapshotTagValuesAsync(request).Result;
            return result[dataSourceName].Values.FirstOrDefault().NumericValue;
        }
    }
}
