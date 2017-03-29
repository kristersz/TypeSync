using System.Text;
using log4net;
using Microsoft.CodeAnalysis;
using TypeSync.Core.Constants;
using TypeSync.Core.Extensions;
using TypeSync.Core.Models.CSharp;

namespace TypeSync.Core
{
    public class TypeScriptGenerator
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TypeScriptGenerator));

        public TypeScriptGenerator()
        {
        }

        public string Generate(CSharpClassModel classModel)
        {
            log.Info("DTO generation started");

            var sb = new StringBuilder();

            // imports
            sb.AppendLine();

            // class declaration
            sb.AppendLine("export class " + classModel.Name + " {");

            // properties
            foreach (var property in classModel.Properties)
            {
                var tsType = TypeConverter.ConvertCSharpTypeToTypeScript(property.Type.SpecialType);
                var typeLiteral = TypeConverter.ConvertTypeScriptTypeToLiteral(tsType);

                if (property.Type.SpecialType == SpecialType.None)
                {
                    typeLiteral = property.Type.Name;
                }

                if (property.Type.IsCollection)
                {
                    var elementTsType = TypeConverter.ConvertCSharpTypeToTypeScript(property.Type.ElementType.SpecialType);
                    var elementTypeLiteral = TypeConverter.ConvertTypeScriptTypeToLiteral(elementTsType);

                    if (property.Type.ElementType.SpecialType == SpecialType.None)
                    {
                        elementTypeLiteral = property.Type.ElementType.Name;
                    }

                    typeLiteral = elementTypeLiteral + TypeScriptTypeLiteral.Array;
                }

                sb.AppendLine("\t" + property.Name.PascalToCamelCase() + ": " + typeLiteral + ";");
            }

            sb.AppendLine("}");
            sb.AppendLine();

            log.Info("DTO generation finished");

            return sb.ToString();
        }
    }
}
