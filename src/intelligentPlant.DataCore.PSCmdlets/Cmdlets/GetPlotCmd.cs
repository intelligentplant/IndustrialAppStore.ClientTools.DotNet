using IntelligentPlant.DataCore.Client.Model;
using System.Management.Automation;
using IntelligentPlant.DataCore.PSCmdlets.Tools;
using IntelligentPlant.DataCore.PSCmdlets.Scripts;
using System.Security.Cryptography.X509Certificates;

namespace IntelligentPlant.DataCore.PSCmdlets.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "Plot")]
    public class GetPlotCmd : BaseCmdlet
    {
        /// <summary>
        /// Allow the user to export data optimised for plot format.
        /// </summary>

        [Parameter]
        public string DataSource { get; set; }

        [Parameter]
        public string DataCoreUrl { get; set; }

        [Parameter(Mandatory = true)]
        public string Tag1 { get; set; }

        [Parameter]
        public string Tag2 { get; set; } = string.Empty;

        [Parameter]
        public string StartDate { get; set; }

        [Parameter]
        public string EndDate { get; set; }

        [Parameter(Mandatory = true)]
        public int Interval { get; set; }

        [Parameter]
        public string FilePath { get; set; }

        public string SaveToCsv { get; set; }

        public Dictionary<string,HistoricalTagValues>.ValueCollection TagData { get; set; }

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
            GetPlot getPlot = new GetPlot();
            getPlot.Init(DataCoreUrl);

            TagData = getPlot.GetPlotValues(DataSource, Tag1, Tag2, VerifiedStartDate, VerifiedEndDate, Interval);

            ConsoleOutput.PrintTagData(TagData).Wait();

            ToCsv(FilePath,TagData);
        }
    }
}

