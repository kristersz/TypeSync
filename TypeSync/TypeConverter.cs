using System.Collections.Generic;

namespace TypeSync
{
    public static class TypeConverter
    {
        private static IDictionary<string, string> typeDictionary = new Dictionary<string, string>()
        {
            { "string", "string" },
            { "short", "number" },
            { "int", "number" },
            { "long", "number" },
            { "bool", "boolean" },
            { "DateTime", "Date"}
        };

        public static string ConvertCSharpTypeToTypeScript(string type)
        {
            string tsType;

            if (typeDictionary.TryGetValue(type, out tsType))
            {
                return tsType;
            }

            return type;
        }
    }
}
