
using System.Globalization;
using System.Text.RegularExpressions;

namespace SheetCodes
{
    public static class StringExtensions
    {
        public static string FirstLetterToUpper(this string str)
        {
            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }

        public static string FirstLetterToLower(this string str)
        {
            if (str.Length > 1)
                return char.ToLower(str[0]) + str.Substring(1);

            return str.ToLower();
        }

        public static string RemoveBreakingCharacter(this string value)
        {
            return value.Replace("\"", "").Replace("\\", "");
        }

        public static string ConvertStringToEnumString(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            TextInfo titleCaser = new CultureInfo("en-US", false).TextInfo;
            string result = Regex.Replace(value, "[^a-zA-Z0-9_ ]", "");
            result = Regex.Replace(result, @"^[\d-]*\s*", "");
            result = titleCaser.ToTitleCase(result.FirstLetterToLower());
            result = Regex.Replace(result, " ", "");
            return result;
        }

        public static string CreatePropertyName(this string value)
        {
            string result = Regex.Replace(value, "[^a-zA-Z0-9_]", "");
            result = Regex.Replace(result, @"^[\d-]*\s*", "");
            result = Regex.Replace(result, " ", "");
            result = result.FirstLetterToUpper();
            return result;
        }
    }
}