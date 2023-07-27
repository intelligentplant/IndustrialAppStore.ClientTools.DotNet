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

        [Parameter(Mandatory = true)]
        public string DataSource { get; set; }

        [Parameter]
        public string DataCoreUrl { get; set; }

        [Parameter(Mandatory = true)]
        public string Tag1 { get; set; }

        [Parameter]
        public string Tag2 { get; set; } = string.Empty;

        [Parameter(Mandatory = true)]
        public DateTime StartDate { get; set; }

        [Parameter(Mandatory = true)]
        public DateTime EndDate { get; set; }

        [Parameter(Mandatory = true)]
        public int Interval { get; set; }

        [Parameter]
        public string FilePath { get; set; }

        [Parameter]
        public string SaveToCsv { get; set; }

        public Dictionary<string,HistoricalTagValues>.ValueCollection TagData { get; set; }


        protected override void BeginProcessing()
        {
            if(DataCoreUrl == null)
            {
                DataCoreUrl = ValidateDataCoreUrl();
            }

            if (string.IsNullOrEmpty(Tag2))
            {
                Console.Write("Tag2 (Optional): ");
                string userInput = Console.ReadLine()?.Trim();
                if (!string.IsNullOrEmpty(userInput))
                {
                    Tag2 = userInput;
                }
            }

        }
        protected override void ProcessRecord()
        {
            GetPlot getPlot = new GetPlot();
            getPlot.Init(DataCoreUrl);

            TagData = getPlot.GetPlotValues(DataSource, Tag1, Tag2, StartDate, EndDate,Interval);

            ConsoleOutput.PrintTagData(TagData).Wait();

            ToCsv();

            //if(SaveToCsv == null)
            //{
            //    Console.WriteLine();
            //    Console.Write("Export Data to csv? (y/n): ");
            //    SaveToCsv = Console.ReadLine();
            //}
            //
            //if(SaveToCsv == "y" || SaveToCsv == "Y")
            //{
            //    if(FilePath == null)
            //    {
            //        FilePath = ValidateFilePath();
            //    }
            //    CsvMethods.RawDataToCSv(TagData, FilePath);
            //}   
        }

        public void ToCsv ()
        {
            if (FilePath == null)
            {
                Console.WriteLine();
                Console.Write("Export Data to csv? (y/n): ");
                SaveToCsv = Console.ReadLine();

                if (SaveToCsv == "y" || SaveToCsv == "Y")
                {
                    FilePath = ValidateFilePath();
                }
                else
                {
                    return;
                }
            }
            CsvMethods.RawDataToCSv(TagData, FilePath);
        }
    }
}

