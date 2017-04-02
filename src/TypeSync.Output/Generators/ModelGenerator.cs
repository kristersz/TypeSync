﻿using System.Text;
using log4net;
using TypeSync.Common.Extensions;
using TypeSync.Models.TypeScript;
using TypeSync.Output.Converters;

namespace TypeSync.Output.Generators
{
    public class ModelGenerator
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ModelGenerator));

        public string Generate(TypeScriptClassModel classModel)
        {
            log.Info("Model generation started");

            var sb = new StringBuilder();

            // imports
            sb.AppendLine();

            // class declaration
            sb.AppendLine("export class " + classModel.Name + " {");

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

                sb.AppendLine("\t"                      // indentation
                    + property.Name.PascalToCamelCase() // property name
                    + (property.IsOptional ? "?" : "")  // optional?
                    + ": "
                    + emittedType                       // property type
                    + ";");
            }

            sb.AppendLine("}");
            sb.AppendLine();

            log.Info("Model generation finished");

            return sb.ToString();
        }
    }
}
