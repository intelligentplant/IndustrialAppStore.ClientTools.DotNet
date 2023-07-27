using System.Management.Automation;
using IntelligentPlant.DataCore.PSCmdlets.Scripts;
using IntelligentPlant.DataCore.PSCmdlets.Tools;

namespace IntelligentPlant.DataCore.PSCmdlets.Cmdlets
{
    [Cmdlet(VerbsCommon.Add, "ScriptTags")]
    public class AddScriptTagsCmd : BaseCmdlet
    {
        /// <summary>
        /// An implementation of Upload Script Tag Definitions
        /// 
        /// -- allows the user to perfrom a bulk upload of script tags, related
        ///     to a certian template.
        /// </summary>


        public DateTime DateTime = DateTime.UtcNow.AddDays(0);

        [Parameter]
        public string Datasource { get; set; }

        [Parameter]
        public string DataCoreUrl { get; set; }

        [Parameter]
        public required string FilePath { get; set; }

        [Parameter]
        public string ScriptEngine = ScriptEngineType.CSharp;

        protected override void BeginProcessing()
        {
            if (Datasource == null)
            {
                Console.Write("Data Source: ");
                Datasource = Console.ReadLine();
            }
            if (DataCoreUrl == null)
            {
                DataCoreUrl = ValidateDataCoreUrl();
            }
            if (FilePath == null)
            {
                Console.Write("File Path: ");
                FilePath = Console.ReadLine();
            }
        }
        protected override void ProcessRecord()
        {   
            UploadScriptTagDefinitions uploadScriptTagDefinitions = new UploadScriptTagDefinitions();
            uploadScriptTagDefinitions.Init(DataCoreUrl);

            var csvData = CsvMethods.ReadCSV(FilePath).Result;
            uploadScriptTagDefinitions.UploadScriptTagDefenitions(Datasource, ScriptEngine, DateTime, csvData).Wait();
        }
        
    }
}
