using IntelligentPlant.DataCore.Client.Queries;
using IntelligentPlant.DataCore.Client.Model;
using IntelligentPlant.DataCore.Client;
using Newtonsoft.Json;
using IntelligentPlant.DataCore.Client.Model.Scripting;
using System.Data;
using IntelligentPlant.DataCore.PSCmdlets.Tools;

namespace IntelligentPlant.DataCore.PSCmdlets.Scripts
{
    public class GetScriptTagDefinitions
    {
        private DataCoreHttpClient dataCoreClient;

        public void Init(string datacoreUrl)
        {
            var httpclient = GetHttpClient.Instance.CreateHttpClient(datacoreUrl);

            dataCoreClient = GetDataCoreHttpClient.Instance.CreateDataCoreClient(httpclient, datacoreUrl);
        }

        public async Task ExportScriptTagDefinitions(string datasource, string scriptEngineType, string templateId, string nameFilter, string filePath)
        {
            const int pageSize = 20;
            var page = 1;
            var doSearch = true;
            var scriptTagsForExport = new List<ScriptTagDefinition>();
        
            //Get script tags
            while (doSearch)
            {
                var findTagsRequest = new FindTagsRequest
                {
                    DataSourceName = datasource,
                    Filter = new TagSearchFilter(nameFilter) { Page = page, PageSize = pageSize }
                };
        
                var scriptTags = await dataCoreClient.DataSources.FindScriptTagsAsync(findTagsRequest).ConfigureAwait(false);   
        
                if(scriptTags.Count() == pageSize)
                {
                    page++;
                }
                else
                {
                    doSearch = false;
                }
                foreach(var scriptTag in scriptTags)
                {
                    if (!scriptTag.Properties.ContainsKey("template.name")) { continue; }
                    var templateName = scriptTag.Properties["template.name"];
                    if(string.Equals(templateName,templateId, StringComparison.OrdinalIgnoreCase))
                    {
                        scriptTagsForExport.Add(scriptTag);
                    }
                }
            }
            if(scriptTagsForExport.Count == 0)
            {
                Console.WriteLine("Error: No Script Tags found");
            }
        
            //Get ScriptTag Template
            var getScriptTagTemplateRequest = new GetScriptTagTemplateRequest
            {
                DataSourceName = datasource,
                ScriptEngineId = scriptEngineType,
                TemplateId = templateId,
            };
        
            var scriptTagTemplate = await dataCoreClient.DataSources.GetScriptTagTemplateAsync(getScriptTagTemplateRequest).ConfigureAwait(false);
        
            if(scriptTagTemplate == null)
            {
                Console.WriteLine("No script-tag template found!");
                return;
            }
        
            var dt = new DataTable();
        
            //prepare headers
            dt.Columns.Add("TemplateName", typeof(string));
            dt.Columns.Add("ScriptTagName", typeof(string));
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("Units", typeof(string));
            dt.Columns.Add("IsEnabled", typeof(string));
            dt.Columns.Add("IsArchivingEnabled", typeof(string));
            dt.Columns.Add("ArchivingDataSourceName", typeof(string));
        
        
            foreach(var param in scriptTagTemplate.Parameters)
            {
                if(param.Type == SimpleType.tagReference)
                {
                    dt.Columns.Add($"{param.Name}.dsn", typeof(string));
                    dt.Columns.Add($"{param.Name}.tag", typeof(string));
                    //dt.Columns.Add($"{param.Name}.requiresNewerValue", typeof(string));
                }
                else
                {
                    dt.Columns.Add(param.Name, typeof(string));
                }
            }
        
            //Process Script Tag Definitions
            foreach(var scriptTag in scriptTagsForExport)
            {
                var row = dt.NewRow();
                row["TemplateName"] = templateId;
                row["ScriptTagName"] = scriptTag.Name;
                row["Description"] = scriptTag.Description;
                row["Units"] = scriptTag.Units;
                row["IsEnabled"] = scriptTag.IsEnabled;
                row["IsArchivingEnabled"] = scriptTag.ArchiveSettings.IsArchivingEnabled;
                row["ArchivingDataSourceName"] = scriptTag.ArchiveSettings.ArchivingDataSourceName;
        
                foreach(var prop in scriptTag.Properties)
                {
                    var shortPropKey = prop.Key.Split('.').Last();
                    //check if property is for tag ref
                    if(prop.Value.Contains("\"dsn\""))
                        try
                        {
                            TagReference tagref = JsonConvert.DeserializeObject<TagReference>(prop.Value);
                            var propName = prop.Key.Split(".").Last();
                            row[$"{propName}.dsn"] = tagref.DataSourceName;
                            row[$"{propName}.tag"] = tagref.TagName;
                            //row[$"{propName}.requiresNewerValue"] = tagref.RequiresNewerValue;
                        }
                        catch
                        {
                            Console.WriteLine("Unable to parse what looks like a tagret property");
                            Console.WriteLine(prop.Value);
                        }
                    else if (dt.Columns.Contains(shortPropKey))
                    {
                        row[shortPropKey] = prop.Value;
                    }
                }
                dt.Rows.Add(row);
            }
        
            CsvMethods.DataTableToCSV(dt, filePath);
        }
    } 
}
