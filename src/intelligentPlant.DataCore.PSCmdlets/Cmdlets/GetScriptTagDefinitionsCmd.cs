using IntelligentPlant.DataCore.PSCmdlets.Scripts;
using IntelligentPlant.DataCore.PSCmdlets.Tools;
using System.Management.Automation;

namespace IntelligentPlant.DataCore.PSCmdlets.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "ScriptTagDefinitions")]
    public class GetScriptTagDefinitionsCmd : BaseCmdlet
    {
        /// <summary>
        /// An implementation of Export Script Tag Definitions
        /// 
        /// -- Allows the user to download a csv of the configured script tags relating to a 
        /// specific templaete
        /// </summary>

        [Parameter]
        public required string DataSource;

        [Parameter]
        public required string DataCoreUrl; 

        [Parameter]
        public string ScriptEngine = ScriptEngineType.CSharp;

        [Parameter]
        public string NameFilter;

        [Parameter]
        public required string FilePath { get; set; }

        [Parameter]
        public required string TemplateId { get; set; }

        protected override void BeginProcessing()
        {
            DataCoreUrl = ValidateDataCoreUrl(DataCoreUrl);
            DataSource = ValidateDataSource(DataSource);

            var ValidTemplates = GetValidTemplates.GetScriptTagTemplatesList(DataSource, ScriptEngine, DataCoreUrl).Result;

            FilePath = ValidateFilePath(FilePath);
            TemplateId = ValidateTemplateId(ValidTemplates, TemplateId);

            if (NameFilter == null)
            {
                Console.Write("Name Filter (* for all): ");
                NameFilter = Console.ReadLine();
            }

            if(NameFilter == null) {
                NameFilter = "*";   //default value
            }
        }

        protected override void ProcessRecord()
        { 
            GetScriptTagDefinitions getScriptTagDefinitions = new GetScriptTagDefinitions();
            getScriptTagDefinitions.Init(DataCoreUrl);

            getScriptTagDefinitions.ExportScriptTagDefinitions(DataSource, ScriptEngine, TemplateId, NameFilter, FilePath).Wait();
        }
    }
}



