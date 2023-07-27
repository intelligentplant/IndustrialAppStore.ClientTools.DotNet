using IntelligentPlant.DataCore.PSCmdlets.Tools;
using IntelligentPlant.DataCore.PSCmdlets.Scripts;
using System.Management.Automation;
using IntelligentPlant.DataCore.Client.Model.Scripting;

namespace IntelligentPlant.DataCore.PSCmdlets.Cmdlets
{

    [Cmdlet(VerbsCommon.Get, "Snapshot")]
    public class GetSnapshotCmd : BaseCmdlet
    {
        // Declare the parameters for the cmdlet.
        [Parameter]
        public string DataCoreUrl { get; set; }

        [Parameter(Mandatory = true)]
        public string DataSource { get; set; }

        [Parameter(Mandatory = true)]
        public string TagName { get; set; }

        protected override void BeginProcessing()
        {
            if (DataCoreUrl == null)
            {
                DataCoreUrl = ValidateDataCoreUrl();
            }
            if (DataSource == null)
            {
                Console.Write("Data Source: ");
                DataSource = Console.ReadLine();
            }
        }
        protected override void ProcessRecord()
        { 

            GetSnapshot getSnapshot = new GetSnapshot();
            getSnapshot.Init(DataCoreUrl);

            var result = getSnapshot.ReadSnapshotNumeric(DataCoreUrl, TagName);
            Console.WriteLine(result);
        }
    }
}