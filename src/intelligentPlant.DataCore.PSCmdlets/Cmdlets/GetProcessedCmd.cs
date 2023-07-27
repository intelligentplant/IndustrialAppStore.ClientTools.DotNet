using IntelligentPlant.DataCore.PSCmdlets.Scripts;
using IntelligentPlant.DataCore.PSCmdlets.Tools;
using IntelligentPlant.DataCore.Client.Model;
using System.Management.Automation;


namespace IntelligentPlant.DataCore.PSCmdlets.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "Processed")]
    public class GetProcessedCmd:BaseCmdlet
    {

        [Parameter(Mandatory = true)]
        public string DataSource { get; set; }

        [Parameter]
        public string DataCoreUrl { get; set; }

        [Parameter(Mandatory = true)]
        public string Tag1 { get; set; }

        [Parameter(Mandatory = true)]
        public DateTime StartDate { get; set; }

        [Parameter(Mandatory = true)]
        public DateTime EndDate { get; set; }

        [Parameter]
        public string DataFunction { get; set; }

        [Parameter(Mandatory = true)]
        public TimeSpan Interval { get; set; }

        [Parameter]
        public string FilePath { get; set; }

        public Dictionary<string,HistoricalTagValues> TagData { get; set; }

        protected override void BeginProcessing()
        {
            if(DataCoreUrl == null)
            {
                DataCoreUrl = ValidateDataCoreUrl();
            }
            if(DataFunction == null)
            {
                DataFunction = ValidateDataFunction();
            }
        }


        protected override void ProcessRecord()
        {
            GetProcessed getProcessed = new GetProcessed();
            getProcessed.Init(DataCoreUrl);

            var TagData = getProcessed.GetProcessedValues(DataSource, Tag1, StartDate, EndDate, DataFunction, Interval);

            ConsoleOutput.PrintTagData(TagData).Wait();

            Console.WriteLine();
            Console.Write("Export Data to CSV? (y/n): ");
            var saveFile = Console.ReadLine();

            if(saveFile == "y" || saveFile == "Y")
            {
                FilePath = ValidateFilePath();
                CsvMethods.RawDataToCSv(TagData, FilePath);
            }
        }
    }
}
