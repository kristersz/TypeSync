using TypeSync.Models.Enums;

namespace TypeSync.Output.Converters
{
    public static class TypeMapper
    {
        public static TypeScriptType MapCSharpTypeToTypeScript(CSharpSpecialType type)
        {
            switch (type)
            {
                case CSharpSpecialType.None:
                case CSharpSpecialType.System_Object:
                    return TypeScriptType.Any;

                case CSharpSpecialType.System_Enum:
                    return TypeScriptType.Enum;

                case CSharpSpecialType.System_MulticastDelegate:
                case CSharpSpecialType.System_Delegate:
                case CSharpSpecialType.System_ValueType:
                    return TypeScriptType.Any;

                case CSharpSpecialType.System_Void:
                    return TypeScriptType.Void;

                case CSharpSpecialType.System_Boolean:
                    return TypeScriptType.Boolean;

                case CSharpSpecialType.System_Char:
                    return TypeScriptType.String;

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
                    return TypeScriptType.Number;

                case CSharpSpecialType.System_String:
                    return TypeScriptType.String;

                case CSharpSpecialType.System_IntPtr:
                case CSharpSpecialType.System_UIntPtr:
                    return TypeScriptType.Any;

                case CSharpSpecialType.System_Array:
                case CSharpSpecialType.System_Collections_IEnumerable:
                case CSharpSpecialType.System_Collections_Generic_IEnumerable_T:
                case CSharpSpecialType.System_Collections_Generic_IList_T:
                case CSharpSpecialType.System_Collections_Generic_ICollection_T:
                case CSharpSpecialType.System_Collections_IEnumerator:
                case CSharpSpecialType.System_Collections_Generic_IEnumerator_T:
                case CSharpSpecialType.System_Collections_Generic_IReadOnlyList_T:
                case CSharpSpecialType.System_Collections_Generic_IReadOnlyCollection_T:
                    return TypeScriptType.Array;

                case CSharpSpecialType.System_Nullable_T:
                    return TypeScriptType.Any;

                case CSharpSpecialType.System_DateTime:
                    return TypeScriptType.Date;

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
                    return TypeScriptType.Any;

                default:
                    return TypeScriptType.Any;
            }
        }

        public static string MapTypeScriptTypeToLiteral(TypeScriptType type)
        {
            switch (type)
            {
                case TypeScriptType.Any:
                    return EmittedTypeName.Any;
                case TypeScriptType.Boolean:
                    return EmittedTypeName.Boolean;
                case TypeScriptType.Number:
                    return EmittedTypeName.Number;
                case TypeScriptType.String:
                    return EmittedTypeName.String;
                case TypeScriptType.Array:
                    return EmittedTypeName.Array;
                case TypeScriptType.Tuple:
                    return EmittedTypeName.Tuple;
                case TypeScriptType.Enum:
                    return EmittedTypeName.Enum;
                case TypeScriptType.Void:
                    return EmittedTypeName.Void;
                case TypeScriptType.Null:
                    return EmittedTypeName.Null;
                case TypeScriptType.Undefined:
                    return EmittedTypeName.Undefined;
                case TypeScriptType.Never:
                    return EmittedTypeName.Never;
                case TypeScriptType.Date:
                    return EmittedTypeName.Date;
                default:
                    return EmittedTypeName.Any;
            }
        }
    }
}
