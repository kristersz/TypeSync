using TypeSync.Models.CSharp;
using TypeSync.Models.TypeScript;

namespace TypeSync.Output.Converters
{
    public static class TypeMapper
    {
        public static TypeScriptBasicType MapCSharpTypeToTypeScript(CSharpSpecialType type)
        {
            switch (type)
            {
                case CSharpSpecialType.None:
                case CSharpSpecialType.System_Object:
                    return TypeScriptBasicType.Any;

                case CSharpSpecialType.System_Enum:
                    return TypeScriptBasicType.Enum;

                case CSharpSpecialType.System_MulticastDelegate:
                case CSharpSpecialType.System_Delegate:
                case CSharpSpecialType.System_ValueType:
                    return TypeScriptBasicType.Any;

                case CSharpSpecialType.System_Void:
                    return TypeScriptBasicType.Void;

                case CSharpSpecialType.System_Boolean:
                    return TypeScriptBasicType.Boolean;

                case CSharpSpecialType.System_Char:
                    return TypeScriptBasicType.String;

                case CSharpSpecialType.System_SByte:
                case CSharpSpecialType.System_Byte:
                case CSharpSpecialType.System_Int16:
                case CSharpSpecialType.System_UInt16:
                case CSharpSpecialType.System_Int32:
                case CSharpSpecialType.System_UInt32:
                case CSharpSpecialType.System_Int64:
                case CSharpSpecialType.System_UInt64:
                case CSharpSpecialType.System_Decimal:
                case CSharpSpecialType.System_Single:
                case CSharpSpecialType.System_Double:
                    return TypeScriptBasicType.Number;

                case CSharpSpecialType.System_String:
                    return TypeScriptBasicType.String;

                case CSharpSpecialType.System_IntPtr:
                case CSharpSpecialType.System_UIntPtr:
                    return TypeScriptBasicType.Any;

                case CSharpSpecialType.System_Array:
                case CSharpSpecialType.System_Collections_IEnumerable:
                case CSharpSpecialType.System_Collections_Generic_IEnumerable_T:
                case CSharpSpecialType.System_Collections_Generic_IList_T:
                case CSharpSpecialType.System_Collections_Generic_ICollection_T:
                case CSharpSpecialType.System_Collections_IEnumerator:
                case CSharpSpecialType.System_Collections_Generic_IEnumerator_T:
                case CSharpSpecialType.System_Collections_Generic_IReadOnlyList_T:
                case CSharpSpecialType.System_Collections_Generic_IReadOnlyCollection_T:
                    return TypeScriptBasicType.Array;

                case CSharpSpecialType.System_Nullable_T:
                    return TypeScriptBasicType.Any;

                case CSharpSpecialType.System_DateTime:
                    return TypeScriptBasicType.Date;

                case CSharpSpecialType.System_Runtime_CompilerServices_IsVolatile:
                case CSharpSpecialType.System_IDisposable:
                case CSharpSpecialType.System_TypedReference:
                case CSharpSpecialType.System_ArgIterator:
                case CSharpSpecialType.System_RuntimeArgumentHandle:
                case CSharpSpecialType.System_RuntimeFieldHandle:
                case CSharpSpecialType.System_RuntimeMethodHandle:
                case CSharpSpecialType.System_RuntimeTypeHandle:
                case CSharpSpecialType.System_IAsyncResult:
                case CSharpSpecialType.System_AsyncCallback:
                    return TypeScriptBasicType.Any;

                default:
                    return TypeScriptBasicType.Any;
            }
        }

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
