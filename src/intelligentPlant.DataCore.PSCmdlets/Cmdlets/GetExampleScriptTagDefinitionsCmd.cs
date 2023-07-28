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
            DataSource = ValidateDataSource(DataSource);
            DataCoreUrl = ValidateDataCoreUrl(DataCoreUrl);

            var ValidTemplates = GetValidTemplates.GetScriptTagTemplatesList(DataSource, ScriptEngine, DataCoreUrl).Result;

            TemplateId = ValidateTemplateId(ValidTemplates, TemplateId);
            FilePath = ValidateFilePath(FilePath);
        }

        protected override void ProcessRecord()
        {   
            
            GetExampleScriptTagDefinitions getExampleScriptTagDefinitions = new GetExampleScriptTagDefinitions();
            getExampleScriptTagDefinitions.Init(DataCoreUrl);

            getExampleScriptTagDefinitions.DownloadExampleScriptTagDefinitions(DataSource, ScriptEngine, TemplateId, FilePath).Wait();
            WriteObject("File saved sucessfully!");
        }
    }
}
