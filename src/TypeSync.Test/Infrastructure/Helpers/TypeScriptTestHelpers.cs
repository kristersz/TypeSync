using System.Collections.Generic;
using System.Text;

namespace TypeSync.Test.Infrastructure.Helpers
{
    public static class TypeScriptTestHelpers
    {
        public static string BuildTypeScriptClass(string className, List<KeyValuePair<string, string>> fields)
        {
            var sb = new StringBuilder();

            sb.AppendLine("export class " + className + " {");

            foreach (var field in fields)
            {
                sb.AppendLine("\t" + field.Key + ": " + field.Value + ";");
            }

            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
