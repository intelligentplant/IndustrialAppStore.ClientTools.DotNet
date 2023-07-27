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
        public required string DatacoreUrl; 

        [Parameter]
        public string ScriptEngine = ScriptEngineType.CSharp;

        [Parameter]
        public string NameFilter = "*"; //defualt value

        [Parameter]
        public required string FilePath { get; set; }

        [Parameter]
        public required string TemplateId { get; set; }

        protected override void BeginProcessing()
        {
            if (DatacoreUrl == null)
            {
                DatacoreUrl = ValidateDataCoreUrl();
            }
            if (DataSource == null)
            {
                Console.Write("Data Source: ");
                DataSource = Console.ReadLine();
            }

            var ValidTemplates = GetValidTemplates.GetScriptTagTemplatesList(DataSource, ScriptEngine, DatacoreUrl).Result;

            if (FilePath == null)
            {
                FilePath = ValidateFilePath();
            }
            if (TemplateId == null)
            {
                TemplateId = ValidateTemplateId(ValidTemplates);
            }
            if (NameFilter == null)
            {
                Console.Write("Name Filter (* for all): ");
                NameFilter = Console.ReadLine();
            }
        }

        protected override void ProcessRecord()
        { 
            GetScriptTagDefinitions getScriptTagDefinitions = new GetScriptTagDefinitions();
            getScriptTagDefinitions.Init(DatacoreUrl);

            getScriptTagDefinitions.ExportScriptTagDefinitions(DataSource, ScriptEngine, TemplateId, NameFilter, FilePath).Wait();
        }
    }
}



