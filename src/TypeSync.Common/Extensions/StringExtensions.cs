using System;
using System.Text.RegularExpressions;

namespace TypeSync.Common.Extensions
{
    public static class StringExtensions
    {
        public static string PascalToKebabCase(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            return Regex.Replace(str, "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", "-$1", RegexOptions.Compiled).Trim().ToLower();
        }

        public static string PascalToCamelCase(this string str)
        {
            return Char.ToLowerInvariant(str[0]) + str.Substring(1);
        }
    }
}
