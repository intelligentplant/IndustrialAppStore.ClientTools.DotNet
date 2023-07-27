using IntelligentPlant.DataCore.Client;
using IntelligentPlant.DataCore.Client.Model;
using IntelligentPlant.DataCore.Client.Queries;
using IntelligentPlant.DataCore.PSCmdlets.Tools;

namespace IntelligentPlant.DataCore.PSCmdlets.Scripts
{
    public class GetPlot
    {
        private static DataCoreHttpClient dataCoreClient;

        public void Init(string dataCoreUrl)
        {
            var httpclient = GetHttpClient.Instance.CreateHttpClient(dataCoreUrl);

            dataCoreClient = GetDataCoreHttpClient.Instance.CreateDataCoreClient(httpclient, dataCoreUrl);
        }

        public Dictionary<string,HistoricalTagValues>.ValueCollection GetPlotValues(string dataSoureName, string tag1, string tag2, DateTime startDate, DateTime endDate, int interval)
        {
            var tags = new Dictionary<string, string[]>
            {
                { dataSoureName, new string[] {tag1,tag2} }
                //Multiple data sources can be included here
            };

            var request = new ReadPlotTagValuesRequest
            {
                Tags = tags,
                StartTime = startDate,
                EndTime = endDate,
                Intervals = interval
            };

            var result = dataCoreClient.DataSources.ReadPlotTagValuesAsync(request).Result;

            return result[dataSoureName].Values;
        }
    }
}
