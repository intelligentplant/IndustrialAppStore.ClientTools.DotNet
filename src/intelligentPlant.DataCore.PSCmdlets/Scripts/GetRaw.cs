using IntelligentPlant.DataCore.Client;
using IntelligentPlant.DataCore.Client.Model;
using IntelligentPlant.DataCore.Client.Queries;
using IntelligentPlant.DataCore.PSCmdlets.Tools;

namespace IntelligentPlant.DataCore.PSCmdlets.Scripts
{
    public class GetRaw
    {
        private static DataCoreHttpClient dataCoreClient;

        public void Init(string dataCoreUrl)
        {
            var httpclient = GetHttpClient.Instance.CreateHttpClient(dataCoreUrl);

            dataCoreClient = GetDataCoreHttpClient.Instance.CreateDataCoreClient(httpclient, dataCoreUrl);
        }

        public Dictionary<string, HistoricalTagValues>.ValueCollection ReadRawValues(string dataSouceName, string tag1, int samples, string tag2, DateTime startDate, DateTime endDate)
        {
            var tags = new Dictionary<string, string[]>
            {
                {dataSouceName, new string[] {tag1,tag2} }
            };

            var request = new ReadRawTagValuesRequest
            {
                Tags = tags,
                PointCount = samples,
                StartTime = startDate,
                EndTime = endDate
            };

            var result = dataCoreClient.DataSources.ReadRawTagValuesAsync(request).Result;

            return result[dataSouceName].Values;
        }
    }
}
