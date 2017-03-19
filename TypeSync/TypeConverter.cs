using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeSync
{
    public static class TypeConverter
    {
        private static IDictionary<string, string> typeDictionary = new Dictionary<string, string>()
        {
            { "int", "number" },
            { "long", "number" },
            { "short", "number" },
            { "string", "string" },
            { "bool", "boolean" },
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
