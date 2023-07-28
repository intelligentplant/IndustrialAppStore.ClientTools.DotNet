using System.Text.RegularExpressions;

namespace IntelligentPlant.DataCore.PSCmdlets.Tools
{
    public class Validations
    {
        /// <summary>
        /// Provide Validations for user inputs
        /// </summary>s

        public static bool ValidateUrl(string url)
        {
            return Uri.IsWellFormedUriString(url,UriKind.Absolute);
        }

        public static int ValidateTemplate(string stringToValidate, string[] validInputs)
        {
            if (stringToValidate.Equals("?"))
            {
                Console.WriteLine();
                //the user has requested a list of the valid inputs
                for(int i = 0; i < validInputs.Length; i++)
                {
                    Console.WriteLine(validInputs[i]);
                }
                Console.WriteLine();
                return 2;
            }
            else if(!Array.Exists(validInputs, t=> t.Equals(stringToValidate, StringComparison.OrdinalIgnoreCase)))
            {
                //the template is invalid
                Console.WriteLine("For a list of valid inputs hit '?'" );
                return 1;
            }
            else
            {
                //the template is valid
                return 0;
            }
        }
    }
}
