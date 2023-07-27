using IntelligentPlant.DataCore.Client.Queries;
using IntelligentPlant.DataCore.Client.Model.Scripting.Templates;
using IntelligentPlant.DataCore.Client.Model.Scripting;
using IntelligentPlant.DataCore.Client.Model;
using IntelligentPlant.DataCore.Client;
using System.Data;
using System.Net.Http.Formatting;
using IntelligentPlant.DataCore.PSCmdlets.Tools;

namespace IntelligentPlant.DataCore.PSCmdlets.Scripts
{

    public class UploadScriptTagDefinitions
    {
        private readonly List<string> _uploadedTags = new List<string>();

        private DataCoreHttpClient dataCoreClient;


        public void Init(string datacoreUrl)
        {
            var httpclient = GetHttpClient.Instance.CreateHttpClient(datacoreUrl);

            dataCoreClient = GetDataCoreHttpClient.Instance.CreateDataCoreClient(httpclient, datacoreUrl);
        }

        public async Task UploadScriptTagDefenitions(string datasource, string scriptEngineId, DateTime? initialSampleTime, (string[] fields, List<string[]> csvData) data)
        {
            var (fields, csvData) = data;

            foreach(var record in csvData)
            {
                //prepare request
                var scriptTagRequest = new CreateTemplatedScriptTagRequest
                {
                    DataSourceName = datasource,
                    Settings = new CreateTemplatedScriptTagSettings
                    {
                        ApplicationName = "Data Core",
                        ScriptEngineId = scriptEngineId,
                        TemplateParameters = new Dictionary<string, object>(),
                        InitialSampleTime = initialSampleTime,
                        ArchiveSettings = new ScriptTagArchiveSettings
                        {
                            IsArchivingEnabled = false
                        }
                    }
                };

                var templateParameters = new Dictionary<string, object>();
                var tagRefs = new Dictionary<string, TagReference>();
                for (int i = 0; i < fields.Length; i++)
                {
                    var field = fields[i];
                    var value = record[i];

                    //expected fields
                    if (field == "TemplateName")
                    {
                        scriptTagRequest.Settings.TemplateName = value;
                    }
                    if (field == "ScriptTagName")
                    {
                        scriptTagRequest.Settings.Name = value;
                        scriptTagRequest.Settings.ArchiveSettings.ArchivingTagName = value.Replace("$$", ""); //Always give archive tag same name as ScriptTag
                    }
                    else if (field == "Description")
                    {
                        scriptTagRequest.Settings.Description = value;
                    }
                    else if (field == "Units")
                    {
                        scriptTagRequest.Settings.Units = value;
                    }
                    else if (field == "IsEnabled")
                    {
                        scriptTagRequest.Settings.IsEnabled = bool.Parse(value);
                    }
                    else if (field == "IsArchivingEnabled")
                    {
                        scriptTagRequest.Settings.ArchiveSettings.IsArchivingEnabled = bool.Parse(value);
                    }
                    else if (field == "ArchivingDataSourceName")
                    {
                        scriptTagRequest.Settings.ArchiveSettings.ArchivingDataSourceName = value;
                    }
                    else if (field.EndsWith(".dsn") || field.EndsWith(".tag") || field.EndsWith(".requiresNewerValue") || field.EndsWith(".preFetchData"))
                    {
                        var tagRefParts = field.Split(".");
                        if (!tagRefs.ContainsKey(tagRefParts[0]))
                        {
                            tagRefs.Add(tagRefParts[0], new TagReference());
                        }
                        if (field.EndsWith(".dsn") && !string.IsNullOrWhiteSpace(value))
                        {
                            ((TagReference)tagRefs[tagRefParts[0]]).DataSourceName = value;
                        }
                        else if (field.EndsWith(".tag") && !string.IsNullOrWhiteSpace(value))
                        {
                            ((TagReference)tagRefs[tagRefParts[0]]).TagName = value;
                        }
                        else if (field.EndsWith(".requiresNewerValue") && !string.IsNullOrWhiteSpace(value) && bool.TryParse(value, out var requiresNewerValue))
                        {
                            ((TagReference)tagRefs[tagRefParts[0]]).RequiresNewerValue = requiresNewerValue;
                        }
                        else if (field.EndsWith(".preFetchData") && !string.IsNullOrEmpty(value) && bool.TryParse(value, out var preFetchData))
                        {
                            ((TagReference)tagRefs[tagRefParts[0]]).PreFetchData = preFetchData;
                        }
                    }
                    else
                    {
                        templateParameters.Add(field, value);
                    }
                }

                //remove dubious tag-refs where dsn or tagname is missing
                foreach (var tagRef in tagRefs)
                {
                    if (!string.IsNullOrWhiteSpace(tagRef.Value.DataSourceName) && !string.IsNullOrWhiteSpace(tagRef.Value.TagName))
                    {
                        templateParameters.Add(tagRef.Key, tagRef.Value);
                    }
                    else
                    {
                        Console.WriteLine($"Warning: {scriptTagRequest.Settings.Name} Tag Ref \"{tagRef.Key} \" removed as DSN and/or tagName is missing"); //TODO change this so it outputs in the PS window
                    }
                }
                scriptTagRequest.Settings.TemplateParameters = templateParameters;
                await CreateOrUpdateTemplatedScriptTagAsync(scriptTagRequest, CancellationToken.None).ConfigureAwait(false);
            }
        }


        private async Task CreateOrUpdateTemplatedScriptTagAsync(CreateTemplatedScriptTagRequest createRequest, CancellationToken cancellationToken)
        {
            /*
             * Handle create or update of script tag with dupe 
             */

            //check for existing tag
            var findTagsRequest = new FindTagsRequest
            {
                DataSourceName = createRequest.DataSourceName,
                Filter = new TagSearchFilter(createRequest.Settings.Name)
            };

            IEnumerable<ScriptTagDefinition> searchResult;
            using(var request = new HttpRequestMessage(HttpMethod.Post, $"api/configuration/scripting/tags/{Uri.EscapeDataString(createRequest.DataSourceName)}/search"){ Content = new ObjectContent(typeof(TagSearchFilter),findTagsRequest.Filter, new JsonMediaTypeFormatter()) })
            {
                var response = await dataCoreClient.SendAsync(request).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                searchResult = await response.Content.ReadAsAsync<IEnumerable<ScriptTagDefinition>>().ConfigureAwait(false);
            }

            //find the api returns all the tags that match the search filter without the need to specify wildcards
            //therefor we'll filter the results further
            var searchResult2 = searchResult.Where(s => s.Name.Replace("$$", "").Equals(createRequest.Settings.Name.Replace("$$", "")));

            var searchResultCount = searchResult2.Count();

            //It's possible that a created/updated tag may not be avaliable to search imediately 
            //To avoid accidentaly duplicating script tags, we wait then make a recursive call
            if(searchResultCount == 0 && _uploadedTags.Contains(createRequest.Settings.Name))
            {
                Console.WriteLine("$Script tag not avaliable for update - wait and try again...");    //TODO emit this msg from PS
                Thread.Sleep(1000);
                await CreateOrUpdateTemplatedScriptTagAsync(createRequest, cancellationToken).ConfigureAwait(false);
                return;
            }
            if(searchResultCount == 0)
            {
                var result = await dataCoreClient.DataSources.CreateScriptTagFromTemplateAsync(createRequest).ConfigureAwait(false);
                Console.WriteLine($"Created Script Tag {createRequest.Settings.Name}");;
            }
            else if (searchResultCount == 1)
            {
                var id = searchResult2.FirstOrDefault().Id;
                var updateRequest = new UpdateTemplatedScriptTagRequest
                {
                    DataSourceName = createRequest.DataSourceName,
                    ScriptTagId = id,
                    Settings = createRequest.Settings,
                };
                var result = await dataCoreClient.DataSources.UpdateScriptTagFromTemplateAsync(updateRequest).ConfigureAwait(false);
                Console.WriteLine($"Updated script tag {updateRequest.Settings.Name}"); //Move output to PS
            }
            else
            {
                throw new Exception($"Duplicate script tags: {createRequest.Settings.Name} - {searchResultCount} tags");
            }
            if(!_uploadedTags.Contains(createRequest.Settings.Name))
            {
                _uploadedTags.Add(createRequest.Settings.Name);
            }
        }
    }
}
