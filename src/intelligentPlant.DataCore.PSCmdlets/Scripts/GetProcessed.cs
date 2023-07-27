using IntelligentPlant.DataCore.Client;
using IntelligentPlant.DataCore.Client.Model;
using IntelligentPlant.DataCore.Client.Queries;
using IntelligentPlant.DataCore.PSCmdlets.Tools;

namespace IntelligentPlant.DataCore.PSCmdlets.Scripts
{
    public class GetProcessed
    {
        private static DataCoreHttpClient dataCoreClient;

        public void Init(string dataCoreUrl)
        {
            var httpClient = GetHttpClient.Instance.CreateHttpClient(dataCoreUrl);

            dataCoreClient = GetDataCoreHttpClient.Instance.CreateDataCoreClient(httpClient,dataCoreUrl);   
        }

        public Dictionary<string,HistoricalTagValues>.ValueCollection GetProcessedValues(string dataSourceName, string tag1, DateTime startDate, DateTime endDate, string dataFunction, TimeSpan sampleInterval)
        {
            var tags = new Dictionary<string, string[]>
            {
                {dataSourceName, new string[] {tag1} }
            };

            var request = new ReadProcessedTagValuesRequest
            {
                Tags = tags,
                StartTime = startDate,
                EndTime = endDate,
                DataFunction = dataFunction,
                SampleInterval = sampleInterval
            };

            var result = dataCoreClient.DataSources.ReadProcessedTagValuesAsync(request).Result;

            return result[dataSourceName].Values;
        }

    }
}
