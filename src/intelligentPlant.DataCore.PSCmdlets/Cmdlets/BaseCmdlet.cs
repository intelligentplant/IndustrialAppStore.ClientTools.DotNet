using System.Management.Automation;
using System.Text.RegularExpressions;
using IntelligentPlant.DataCore.PSCmdlets.Tools;
using IntelligentPlant.DataCore.Client.Model;

namespace IntelligentPlant.DataCore.PSCmdlets.Cmdlets
{
    public class BaseCmdlet : Cmdlet
    {
        /// <summary>
        /// Provide methods common to all cmdlets.Most of these methods are for validation purposes
        /// 
        /// -- Note on validation, because the cmdlets are intended to work both as a console app and by use of scripts the validation functions are a little quircky. 
        ///     Firstly they check if the value is null, this tells us if the program has been launced in the console, or by an external PS script (in which the variables have been pre-defined)
        ///     If the value is not null then we must verify the parameter is within certian criteria, if it is not, then instead of exiting the cmdlet, we prompt the user for the correct value in the console.
        ///     Note the user can still exit the current cmdlet with ctrl+C
        /// </summary>
        /// 

        protected string ValidateDataCoreUrl(string dataCoreUrl)
        {
            if(dataCoreUrl != null) {
                if (Validations.ValidateUrl(dataCoreUrl)) {
                    return dataCoreUrl;
                }
                else {
                    WriteError(new ErrorRecord(new ArgumentException("Please Enter a Valid URL"), "InvalidURL", ErrorCategory.InvalidArgument, dataCoreUrl));
                }
            }
            
            do
            {
                Console.Write("Data Core Url: ");
                dataCoreUrl = Console.ReadLine().Trim();

                if (Validations.ValidateUrl(dataCoreUrl))
                {
                    return dataCoreUrl;
                }
                else
                {
                    WriteError(new ErrorRecord(new ArgumentException("Please Enter a Valid URL"), "InvalidURL", ErrorCategory.InvalidArgument, dataCoreUrl));
                }
            } while (true);
        }

        protected string ValidateFilePath(string filePath)
        {
            if(filePath != null) {
                if (!Regex.IsMatch(filePath, @"^[A-Za-z]:\\(?:[\w]+\\)*[\w]+\.csv$")) {
                    WriteError(new ErrorRecord(new ArgumentException("Invalid file name/path"), "Invalid Filename", ErrorCategory.InvalidArgument, filePath));
                }
                else {
                    return filePath;
                }
            }

            //Ensure the filepath is valid and file extension is of type .csv
            do
            {
                Console.Write("File Path: ");
                filePath = Console.ReadLine().Trim();

                if (!Regex.IsMatch(filePath, @"^[A-Za-z]:\\(?:[\w]+\\)*[\w]+\.csv$"))
                {
                    WriteError(new ErrorRecord(new ArgumentException("Invalid file name/path"), "Invalid Filename", ErrorCategory.InvalidArgument, filePath));
                }
                else
                {
                    return filePath;
                }
            } while (true);
        }

        protected string ValidateTemplateId(string[] validTemplates, string templateId)
        {
            if(templateId != null) {
                int valid = Validations.ValidateTemplate(templateId, validTemplates);

                if (valid == 0) {
                    //if the template is valid
                    return templateId;
                }
                else if (valid == 1) {
                    //if the template is not valid
                    WriteError(new ErrorRecord(new ArgumentException("Template not found."), "Invalid Template", ErrorCategory.InvalidArgument, templateId));
                }
            }

            do
            {
                Console.Write("Template Id: ");
                templateId = Console.ReadLine().Trim();
                int valid = Validations.ValidateTemplate(templateId, validTemplates);

                if (valid == 0)
                {
                    //if the template is valid
                    return templateId;
                }
                else if (valid == 1)
                {
                    //if the template is not valid
                    WriteError(new ErrorRecord(new ArgumentException("Template not found."), "Invalid Template", ErrorCategory.InvalidArgument, templateId));
                }
            } while (true);
        }

        protected string ValidateDataFunction(string dataFunction)
        {
            //List of valid Data Functions
            string[] validDataFunctions = { "INTERP", "AVG", "MIN", "MAX" };

            if(dataFunction != null) {
                int valid = Validations.ValidateTemplate(dataFunction, validDataFunctions);

                if (valid == 0) {
                    //if the template is valid
                    return dataFunction;
                }
                else if (valid == 1) {
                    //the template is not valid
                    WriteError(new ErrorRecord(new ArgumentException("Invalid Data Function."), "Invalid DataFunction", ErrorCategory.InvalidArgument, dataFunction));
                }
            }

            do
            {
                Console.Write("Data Function: ");
                dataFunction = Console.ReadLine().Trim();
                int valid = Validations.ValidateTemplate(dataFunction, validDataFunctions);

                if (valid == 0)
                {
                    //if the template is valid
                    return dataFunction;
                }
                else if (valid == 1)
                {
                    //the template is not valid
                    WriteError(new ErrorRecord(new ArgumentException("Invalid Data Function."), "Invalid DataFunction", ErrorCategory.InvalidArgument, dataFunction));
                }
            } while (true);
        }

        protected DateTime ValidateDate(string whichDate, string userInputDate) {

            if(userInputDate != null) {
                if(DateTime.TryParse(userInputDate, out DateTime date)) {
                    //Console.WriteLine("Parsed DateTime: " + date);
                    return date;
                }
                else {
                    WriteError(new ErrorRecord(new ArgumentException("Invalid Date/Time"), "Invalid DateTime", ErrorCategory.InvalidArgument, userInputDate));
                }
            }

            do {
                Console.Write(whichDate + " Date: ");
                string startDate = Console.ReadLine();

                if (DateTime.TryParse(startDate, out DateTime date)) {
                    //Console.WriteLine("Parsed DateTime: " + date);
                    return date;
                }
                WriteError(new ErrorRecord(new ArgumentException("Invalid Date/Time"), "Invalid DateTime", ErrorCategory.InvalidArgument, userInputDate));
            } while(true);
        }

        protected TimeSpan ValidateTimeSpan(string userInput) {
            TimeSpan span;

            while (true) {
                if (userInput != null && TimeSpan.TryParse(userInput, out span)) {
                    return span;
                }

                Console.Write("Interval: ");
                var interval = Console.ReadLine();

                if (TimeSpan.TryParse(interval, out span)) {
                    return span;
                }

                WriteError(new ErrorRecord(new ArgumentException("Invalid Interval"), "Invalid TimeSpan", ErrorCategory.InvalidArgument, interval));
            }
        }
        protected string ValidateDataSource(string dataSource) {

            //This isnt really validating, its just making sure there's something

            do {
                if (dataSource == null) {
                    Console.Write("Data Source: ");
                    return dataSource = Console.ReadLine();
                }
            } while (true);

        }

        protected void ToCsv(string filePath, Dictionary<string, HistoricalTagValues>.ValueCollection TagData) {
            if (filePath == null) {
                Console.WriteLine();
                Console.Write("Export Data to csv? (y/n): ");
                var saveToCsv = Console.ReadLine();

                if (saveToCsv == "y" || saveToCsv == "Y") {
                    filePath = ValidateFilePath(filePath);
                }
                else {
                    return;
                }
            }
            CsvMethods.RawDataToCSv(TagData, filePath);
        }
    }
}
