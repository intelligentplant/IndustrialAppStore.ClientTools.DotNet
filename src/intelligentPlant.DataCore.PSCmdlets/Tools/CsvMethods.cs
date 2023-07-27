using IntelligentPlant.DataCore.Client.Model;
using System.Data;
using System.Globalization;
using System.Management.Automation;
using System.Text;

namespace IntelligentPlant.DataCore.PSCmdlets.Tools
{
    public class CsvMethods : Cmdlet
    {
        /// <summary>
        /// Provide methods to save/load data to csv files.
        /// </summary>


        public static async Task<(string[] fields, List<string[]>)> ReadCSV(string filePath)
        {
            List<string[]> csvData = new List<string[]>();
            string[] fields = null;

            try
            {
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvHelper.CsvParser(reader, CultureInfo.InvariantCulture))
                {
                    //read headers
                    fields = csv.Read() ? csv.Record : Array.Empty<string>();

                    //read rows
                    while (csv.Read())
                    {
                        var record = csv.Record;
                        csvData.Add(record);
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Error, File not found: {ex.FileName}");
                //WriteError(new ErrorRecord(new ArgumentException("Template not found"), "Invalid Template", ErrorCategory.InvalidArgument, ex.FileName));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            return (fields, csvData);
        }

        public static void DataTableToCSV(DataTable dtDataTable, string filePath)
        {
            try
            {
                string directoryPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                var csvContent = new StringBuilder();

                // Headers
                csvContent.AppendLine(string.Join(",", dtDataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));

                // Rows
                foreach (DataRow dr in dtDataTable.Rows)
                {
                    var rowValues = dr.ItemArray.Select(field =>
                    {
                        if (field is string stringValue && stringValue.Contains(","))
                        {
                            return $"\"{stringValue.Replace("\"", "\"\"")}\"";
                        }
                        return field.ToString();
                    });

                    csvContent.AppendLine(string.Join(",", rowValues));
                }

                // Write to the file using StreamWriter
                using (StreamWriter sw = new StreamWriter(filePath, false))
                {
                    sw.Write(csvContent.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public static void RawDataToCSv(Dictionary<string,HistoricalTagValues>.ValueCollection tagData, string filePath)
        {
            try
            {
                string directoryPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("TagName,PlotType,Timestamp,Tag Value");

                    foreach (HistoricalTagValues tagValue in tagData)
                    {
                        string tagName = tagValue.TagName;
                        string plotType = tagValue.DisplayType.ToString();

                        foreach (TagValue value in tagValue.Values)
                        {
                            DateTime timeStamp = value.UtcSampleTime;
                            double dataValue = value.NumericValue;
                            writer.WriteLine($"{tagName},{plotType},{timeStamp},{dataValue}");
                        }
                    }
                }
            }catch(Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
