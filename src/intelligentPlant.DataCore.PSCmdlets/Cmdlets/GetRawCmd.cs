using System.Management.Automation;
using System.Reflection.Metadata;
using IntelligentPlant.DataCore.PSCmdlets.Scripts;
using IntelligentPlant.DataCore.PSCmdlets.Tools;

namespace IntelligentPlant.DataCore.PSCmdlets.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "Raw")]
    public class GetRawCmd : BaseCmdlet
    {
        [Parameter]
        public string DataCoreUrl { get; set; }

        [Parameter]
        public required string DataSource { get; set; }

        [Parameter(Mandatory = true)]
        public required string Tag1 { get; set; }

        [Parameter(Mandatory = true)]
        [ValidateRange(0, 100)]
        public required int Samples { get; set; }

        [Parameter(Mandatory = false)]
        public string Tag2 { get; set; } = string.Empty;

        [Parameter(Mandatory = true)]
        public DateTime StartDate { get; set; } = DateTime.UtcNow.AddHours(-1);

        [Parameter(Mandatory = true)]
        public DateTime EndDate { get; set; } = DateTime.UtcNow;

        protected override void BeginProcessing()
        {
            if(DataCoreUrl == null)
            {
                DataCoreUrl = ValidateDataCoreUrl();
            }
        }


        protected override void ProcessRecord()
        {
            

            //if (string.IsNullOrEmpty(Tag2))
            //{
            //    Console.Write("Tag2 (Optional): ");
            //    string userInput = Console.ReadLine()?.Trim();
            //    if (!string.IsNullOrEmpty(userInput))
            //    {
            //        Tag2 = userInput;
            //    }
            //}

            GetRaw getRaw = new GetRaw();
            getRaw.Init(DataCoreUrl);

            var TagData = getRaw.ReadRawValues(DataSource, Tag1, Samples, Tag2, StartDate, EndDate);

            ConsoleOutput.PrintTagData(TagData).Wait();
        }
    }
}
