using System.Collections.Generic;
using System.Text;
using TypeSync.Extensions;
using TypeSync.Models;

namespace TypeSync
{
    public class TypeScriptGenerator
    {
        public string Generate(string className, List<Property> properties)
        {
            var sb = new StringBuilder();

            // imports
            sb.AppendLine();

            // class declaration
            sb.AppendLine("export class " + className + " {");

            // properties
            foreach (var property in properties)
            {
                sb.AppendLine("\t" + property.Name.PascalToCamelCase() + ": " + TypeConverter.ConvertCSharpTypeToTypeScript(property.Type) + ";");
            }

            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
