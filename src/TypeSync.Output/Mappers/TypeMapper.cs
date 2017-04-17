using TypeSync.Models.CSharp;
using TypeSync.Models.TypeScript;
using TypeSync.Output.Emitters;

namespace TypeSync.Output.Converters
{
    public static class TypeMapper
    {
        public static string MapTypeScriptTypeToLiteral(TypeScriptBasicType type)
        {
            switch (type)
            {
                case TypeScriptBasicType.Any:
                    return EmittedTypeName.Any;
                case TypeScriptBasicType.Boolean:
                    return EmittedTypeName.Boolean;
                case TypeScriptBasicType.Number:
                    return EmittedTypeName.Number;
                case TypeScriptBasicType.String:
                    return EmittedTypeName.String;
                case TypeScriptBasicType.Array:
                    return EmittedTypeName.Array;
                case TypeScriptBasicType.Tuple:
                    return EmittedTypeName.Tuple;
                case TypeScriptBasicType.Enum:
                    return EmittedTypeName.Enum;
                case TypeScriptBasicType.Void:
                    return EmittedTypeName.Void;
                case TypeScriptBasicType.Null:
                    return EmittedTypeName.Null;
                case TypeScriptBasicType.Undefined:
                    return EmittedTypeName.Undefined;
                case TypeScriptBasicType.Never:
                    return EmittedTypeName.Never;
                case TypeScriptBasicType.Date:
                    return EmittedTypeName.Date;
                default:
                    return EmittedTypeName.Any;
            }
        }
    }
}
