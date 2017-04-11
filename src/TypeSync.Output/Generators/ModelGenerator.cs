using System.Text;
using log4net;
using TypeSync.Common.Utilities;
using TypeSync.Models.TypeScript;
using TypeSync.Output.Converters;
using TypeSync.Output.Emitters;

namespace TypeSync.Output.Generators
{
    public class ModelGenerator
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ModelGenerator));

        public string GenerateClass(TypeScriptClassModel classModel)
        {
            var sb = new StringBuilder();

            // imports
            foreach (var import in classModel.Imports)
            {
                import.FilePath = NameCaseConverter.ToKebabCase(import.Name);

                sb.AppendLine("import { " + import.Name + " } from './" + import.FilePath + ".model';");
            }

            sb.AppendLine();

            // class declaration
            sb.AppendLine("export class "
                + classModel.Name
                + (classModel.IsGeneric ? $"<{classModel.TypeParameter.Name}>" : "")
                + (string.IsNullOrEmpty(classModel.BaseClass) ? "" : " extends " + classModel.BaseClass)
                + " {");

            // properties
            foreach (var property in classModel.Properties)
            {
                var emittedType = EmittedTypeName.Any;

                if (property.Type.ElementType == null)
                {
                    emittedType = property.Type.IsNamedType 
                        ? property.Type.Name 
                        : TypeMapper.MapTypeScriptTypeToLiteral(property.Type.PredefinedType);
                }
                else
                {
                    emittedType = property.Type.ElementType.IsNamedType 
                        ? property.Type.ElementType.Name 
                        : TypeMapper.MapTypeScriptTypeToLiteral(property.Type.ElementType.PredefinedType);
                    emittedType += EmittedTypeName.Array;
                }

                sb.AppendLine("\t"                                  // indentation
                    + NameCaseConverter.ToCamelCase(property.Name)  // property name
                    + (property.IsOptional ? "?" : "")              // optional?
                    + ": "
                    + emittedType                                   // property type
                    + ";");
            }

            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
        }

        public string GenerateEnum(TypeScriptEnumModel enumModel)
        {
            var sb = new StringBuilder();

            // imports
            sb.AppendLine();

            // enum declaration
            sb.AppendLine("export enum " + enumModel.Name + " {");

            // members
            foreach (var member in enumModel.Members)
            {
                sb.AppendLine("\t"                                              // indentation
                    + member.Name                                               // member name
                    + (member.Value.HasValue ? $" = {member.Value.Value}" : "") // optional constant value
                    + ",");
            }

            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
