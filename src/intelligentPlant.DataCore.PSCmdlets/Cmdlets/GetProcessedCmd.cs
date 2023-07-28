using IntelligentPlant.DataCore.PSCmdlets.Scripts;
using IntelligentPlant.DataCore.PSCmdlets.Tools;
using IntelligentPlant.DataCore.Client.Model;
using System.Management.Automation;


namespace IntelligentPlant.DataCore.PSCmdlets.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "Processed")]
    public class GetProcessedCmd:BaseCmdlet
    {

        [Parameter]
        public string DataSource { get; set; }

        [Parameter]
        public string DataCoreUrl { get; set; }

        [Parameter(Mandatory = true)]
        public string Tag1 { get; set; }

        [Parameter]
        public string StartDate { get; set; }

        [Parameter]
        public string EndDate { get; set; }

        [Parameter]
        public string DataFunction { get; set; }

        [Parameter]
        public string Interval { get; set; }

        [Parameter]
        public string FilePath { get; set; }

        public string SaveToCsv { get; set; }

        public Dictionary<string,HistoricalTagValues>.ValueCollection TagData { get; set; }

        public DateTime VerifiedStartDate;

        public DateTime VerifiedEndDate;

        public TimeSpan VerifiedInterval;

        protected override void BeginProcessing()
        {
            DataSource = ValidateDataSource(DataSource);
            DataCoreUrl = ValidateDataCoreUrl(DataCoreUrl);
            DataFunction = ValidateDataFunction(DataFunction);
            VerifiedStartDate = ValidateDate("Start", StartDate);
            VerifiedEndDate = ValidateDate("End", EndDate);
            VerifiedInterval = ValidateTimeSpan(Interval);
        }


        protected override void ProcessRecord()
        {
            GetProcessed getProcessed = new GetProcessed();
            getProcessed.Init(DataCoreUrl);

            TagData = getProcessed.GetProcessedValues(DataSource, Tag1, VerifiedStartDate, VerifiedEndDate, DataFunction, VerifiedInterval);

            ConsoleOutput.PrintTagData(TagData).Wait();

            ToCsv(FilePath,TagData);
        }
    }
}
