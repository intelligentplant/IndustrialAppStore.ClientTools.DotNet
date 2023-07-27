using System.Management.Automation;
using System.Text.RegularExpressions;
using IntelligentPlant.DataCore.PSCmdlets.Tools;

namespace IntelligentPlant.DataCore.PSCmdlets.Cmdlets
{
    public class BaseCmdlet : Cmdlet
    {
        /// <summary>
        /// Provide methods common to all cmdlets.
        /// </summary>
        /// 

        protected string ValidateDataCoreUrl()
        {

            do
            {
                Console.Write("Data Core Url: ");
                string dataCoreUrl = Console.ReadLine().Trim();

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

        protected string ValidateFilePath()
        {
            //Ensure the filepath is valid and file extension is of type .csv
            do
            {
                Console.Write("File Path: ");
                string FilePath = Console.ReadLine().Trim();

                if (!Regex.IsMatch(FilePath, @"^[A-Za-z]:\\(?:[\w]+\\)*[\w]+\.csv$"))
                {
                    WriteError(new ErrorRecord(new ArgumentException("Invalid file name/path"), "Invalid Filename", ErrorCategory.InvalidArgument, FilePath));
                }
                else
                {
                    return FilePath;
                }
            } while (true);
        }

        protected string ValidateTemplateId(string[] validTemplates)
        {
            do
            {
                Console.Write("Template Id: ");
                string TemplateId = Console.ReadLine().Trim();
                int valid = Validations.ValidateTemplate(TemplateId, validTemplates);

                if (valid == 0)
                {
                    //if the template is valid
                    return TemplateId;
                }
                else if (valid == 1)
                {
                    //if the template is not valid
                    WriteError(new ErrorRecord(new ArgumentException("Template not found."), "Invalid Template", ErrorCategory.InvalidArgument, TemplateId));
                }
            } while (true);
        }

        protected string ValidateDataFunction()
        {
            //List of valid Data Functions
            string[] ValidDataFunctions = { "INTERP", "AVG", "MIN", "MAX" };

            do
            {
                Console.Write("Data Function: ");
                string DataFunction = Console.ReadLine().Trim();
                int valid = Validations.ValidateTemplate(DataFunction, ValidDataFunctions);

                if (valid == 0)
                {
                    //if the template is valid
                    return DataFunction;
                }
                else if (valid == 1)
                {
                    //the template is not valid
                    WriteError(new ErrorRecord(new ArgumentException("Invalid Data Function."), "Invalid DataFunction", ErrorCategory.InvalidArgument, DataFunction));
                }
            } while (true);
        }
    }
}
