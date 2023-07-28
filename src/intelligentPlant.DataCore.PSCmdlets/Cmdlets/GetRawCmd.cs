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
        public string DataSource { get; set; }

        [Parameter(Mandatory = true)]
        public required string Tag1 { get; set; }

        [Parameter(Mandatory = true)]
        [ValidateRange(0, 100)]
        public required int Samples { get; set; }

        [Parameter(Mandatory = false)]
        public string Tag2 { get; set; } = string.Empty;

        [Parameter]
        public string StartDate { get; set; } 

        [Parameter]
        public string EndDate { get; set; }

        public DateTime VerifiedStartDate;

        public DateTime VerifiedEndDate;

        protected override void BeginProcessing()
        {
            DataSource = ValidateDataSource(DataSource);
            DataCoreUrl = ValidateDataCoreUrl(DataCoreUrl);

            VerifiedStartDate = ValidateDate("Start", StartDate);
            VerifiedEndDate = ValidateDate("End", EndDate);

            //if (string.IsNullOrEmpty(Tag2))
            //{
            //    Console.Write("Tag2 (Optional): ");
            //    string userInput = Console.ReadLine()?.Trim();
            //    if (!string.IsNullOrEmpty(userInput))
            //    {
            //        Tag2 = userInput;
            //    }
            //}
        }

        protected override void ProcessRecord()
        {
            GetRaw getRaw = new GetRaw();
            getRaw.Init(DataCoreUrl);

            var TagData = getRaw.ReadRawValues(DataSource, Tag1, Samples, Tag2, VerifiedStartDate, VerifiedEndDate);
            ConsoleOutput.PrintTagData(TagData).Wait();
        }
    }
}
