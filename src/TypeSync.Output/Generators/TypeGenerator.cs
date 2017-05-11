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
                emittedType = typeModel.IsNamedType
                    ? typeModel.Name
                    : TypeMapper.MapTypeScriptTypeToLiteral(typeModel.PredefinedType);
            }
            else
            {
                emittedType = typeModel.ElementType.IsNamedType
                    ? typeModel.ElementType.Name
                    : TypeMapper.MapTypeScriptTypeToLiteral(typeModel.ElementType.PredefinedType);
                emittedType += EmittedTypeName.Array;
            }

            return emittedType;
        }
    }
}
