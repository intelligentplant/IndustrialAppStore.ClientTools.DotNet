using IntelligentPlant.DataCore.Client.Model;

namespace IntelligentPlant.DataCore.PSCmdlets.Tools
{
    public class ConsoleOutput
    {
        /// <summary>
        /// Provide methods for writing data to the console.
        /// </summary>

        public static async Task PrintTagData(Dictionary<string,HistoricalTagValues>.ValueCollection tagData)
        {
            foreach (HistoricalTagValues tagValue in tagData)
            {
                string tagName = tagValue.TagName;
                string plotType = tagValue.DisplayType.ToString();
                Console.WriteLine("");
                Console.WriteLine("Tag: " + tagName);
                Console.WriteLine("Plot Type: " + plotType);
                Console.WriteLine("");

                foreach (TagValue value in tagValue.Values)
                {
                    DateTime timestamp = value.UtcSampleTime;
                    double dataValue = value.NumericValue;
                    Console.WriteLine("Timestamp: " + timestamp + " Tag Value: " + dataValue);
                }
            }
        }  
    }
}
