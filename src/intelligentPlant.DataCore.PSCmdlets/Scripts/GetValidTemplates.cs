using IntelligentPlant.DataCore.PSCmdlets.Tools;
using IntelligentPlant.DataCore.Client.Queries;

namespace IntelligentPlant.DataCore.PSCmdlets.Scripts
{
    public class GetValidTemplates
    {
        public static async Task<string[]> GetScriptTagTemplatesList(string dataSourceName, string scriptEngineId, string dataCoreUrl)
        {
            var httpclient = GetHttpClient.Instance.CreateHttpClient(dataCoreUrl);

            var dataCoreClient = GetDataCoreHttpClient.Instance.CreateDataCoreClient(httpclient, dataCoreUrl);

            var findScriptTagTemplatesRequest = new FindScriptTagTemplatesRequest
            {
                DataSourceName = dataSourceName,
                ScriptEngineId = scriptEngineId
            };

            var scriptTagTemplates = await dataCoreClient.DataSources.FindScriptTagTemplatesAsync(findScriptTagTemplatesRequest).ConfigureAwait(false);
            var result = scriptTagTemplates.Select(t => t.Name).ToArray();

            return result;
        }
    }
}
