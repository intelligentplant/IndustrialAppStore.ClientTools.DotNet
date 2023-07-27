using IntelligentPlant.DataCore.Client;
using IntelligentPlant.DataCore.Client.Queries;
using IntelligentPlant.DataCore.Client.Model;
using System.Data;
using IntelligentPlant.DataCore.PSCmdlets.Tools;

namespace IntelligentPlant.DataCore.PSCmdlets.Scripts
{
    public class GetExampleScriptTagDefinitions
    {
        private DataCoreHttpClient dataCoreClient;

        public void Init(string datacoreUrl)
        {
            var httpclient = GetHttpClient.Instance.CreateHttpClient(datacoreUrl);

            dataCoreClient = GetDataCoreHttpClient.Instance.CreateDataCoreClient(httpclient, datacoreUrl);

        }

        public async Task DownloadExampleScriptTagDefinitions(string dataSource, string scriptEngine, string templateId, string filePath)
        {
            var getScriptTagTemplateRequest = new GetScriptTagTemplateRequest
            {
                DataSourceName = dataSource,
                ScriptEngineId = scriptEngine,
                TemplateId = templateId,
            };

            var scriptTagTemplate = await dataCoreClient.DataSources.GetScriptTagTemplateAsync(getScriptTagTemplateRequest).ConfigureAwait(false);

            var dt = new DataTable();

            //prepare headers
            dt.Columns.Add("TemplateName", typeof(string));
            dt.Columns.Add("ScriptTagName", typeof(string));
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("Units", typeof(string));
            dt.Columns.Add("IsEnabled", typeof(string));
            dt.Columns.Add("IsArchivingEnabled", typeof(string));
            dt.Columns.Add("ArchivingDataSourceName", typeof(string));

            foreach (var param in scriptTagTemplate.Parameters)
            {
                if (param.Type == SimpleType.tagReference)
                {
                    dt.Columns.Add($"{param.Name}.dsn", typeof(string));
                    dt.Columns.Add($"{param.Name}.tag", typeof(string));
                    dt.Columns.Add($"{param.Name}.requiresNewerValue", typeof(string));
                }
                else
                {
                    dt.Columns.Add(param.Name, typeof(string));
                }
            }
            //prepare sample row 
            for (int i = 0; i < 2; i++)
            {
                var row = dt.NewRow();
                row["TemplateName"] = templateId;
                row["ScriptTagName"] = $"ScriptTag{i + 1}";
                row["Description"] = scriptTagTemplate.Description;
                row["Units"] = "";
                row["IsEnabled"] = "TRUE";
                row["IsArchivingEnabled"] = "TRUE";
                row["ArchivingDataSourceName"] = "Facit Archive Historian";

                foreach (var param in scriptTagTemplate.Parameters)
                {
                    if (param.Type == SimpleType.tagReference)
                    {
                        row[$"{param.Name}.dsn"] = $"{param.Name} data source name";
                        row[$"{param.Name}.tag"] = $"{param.Name} tag name";
                        //row[$"{param.Name}.requiresNewerValue"] = "TRUE";
                    }
                    else
                    {
                        row[param.Name] = param.DefaultValue == null ? null : param.DefaultValue.ToString();
                    }
                }
                dt.Rows.Add(row);
            }
            CsvMethods.DataTableToCSV(dt, filePath);
        }
    }
}
