using System.Management.Automation;
using IntelligentPlant.DataCore.PSCmdlets.Scripts;
using IntelligentPlant.DataCore.PSCmdlets.Tools;

namespace IntelligentPlant.DataCore.PSCmdlets.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "ExampleScriptTagDefinitions")]
    public class GetExampleScriptTagDefinitionsCmd : BaseCmdlet
    {
        /// <summary>
        /// An implementation of Download example script tag definitions
        /// 
        /// -- Allows the user to download an example of script tag defintions, related 
        ///     to a certian template.
        /// </summary>
        /// 

        [Parameter]
        public string DataSource { get; set; }

        [Parameter]
        public string DataCoreUrl { get; set; }

        [Parameter]
        public string TemplateId { get; set; }

        [Parameter]
        public string FilePath { get; set; }

        [Parameter]
        public string ScriptEngine = ScriptEngineType.CSharp;

        protected override void BeginProcessing()
        {
            if (DataSource == null)
            {
                Console.Write("Data Source: ");
                DataSource = Console.ReadLine();
            }
            if (DataCoreUrl == null)
            {
                DataCoreUrl = ValidateDataCoreUrl();
            }

            var ValidTemplates = GetValidTemplates.GetScriptTagTemplatesList(DataSource, ScriptEngine, DataCoreUrl).Result;

            if (TemplateId == null)
            {
                TemplateId = ValidateTemplateId(ValidTemplates);
            }

            if (FilePath == null)
            {
                FilePath = ValidateFilePath();
            }
        }

        protected override void ProcessRecord()
        {   
            
            GetExampleScriptTagDefinitions getExampleScriptTagDefinitions = new GetExampleScriptTagDefinitions();
            getExampleScriptTagDefinitions.Init(DataCoreUrl);

            getExampleScriptTagDefinitions.DownloadExampleScriptTagDefinitions(DataSource, ScriptEngine, TemplateId, FilePath).Wait();
        }
    }
}
