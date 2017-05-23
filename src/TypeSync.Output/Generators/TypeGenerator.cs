using System.Linq;
using TypeSync.Models.TypeScript;
using TypeSync.Output.Converters;
using TypeSync.Output.Emitters;

namespace TypeSync.Output.Generators
{
    public class TypeGenerator
    {
        public string GetEmittedType(TypeScriptTypeModel typeModel)
        {
            var emittedType = EmittedTypeName.Any;

            if (typeModel.ElementType == null)
            {
                if (typeModel.TypeArguments.Any())
                {
                    emittedType = $"{GetTypeLiteral(typeModel)}<{GetTypeLiteral(typeModel.TypeArguments.First())}>";
                }
                else
                {
                    emittedType = GetTypeLiteral(typeModel);
                }
                
            }
            else
            {
                emittedType = GetTypeLiteral(typeModel.ElementType);
                emittedType += EmittedTypeName.Array;
            }

            return emittedType;
        }

        private string GetTypeLiteral(TypeScriptTypeModel typeModel)
        {
            return typeModel.IsNamedType
                    ? typeModel.Name
                    : TypeMapper.MapTypeScriptTypeToLiteral(typeModel.PredefinedType);
        }
    }
}
