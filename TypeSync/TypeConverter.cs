using Microsoft.CodeAnalysis;
using TypeSync.Enums;

namespace TypeSync
{
    public static class TypeConverter
    {
        public static TypeScriptType ConvertCSharpTypeToTypeScript(SpecialType type)
        {
            switch (type)
            {
                case SpecialType.None:
                case SpecialType.System_Object:
                    return TypeScriptType.Any;

                case SpecialType.System_Enum:
                    return TypeScriptType.Enum;

                case SpecialType.System_MulticastDelegate:
                case SpecialType.System_Delegate:
                case SpecialType.System_ValueType:
                    return TypeScriptType.Any;

                case SpecialType.System_Void:
                    return TypeScriptType.Void;

                case SpecialType.System_Boolean:
                    return TypeScriptType.Boolean;

                case SpecialType.System_Char:
                    return TypeScriptType.String;

                case SpecialType.System_SByte:
                case SpecialType.System_Byte:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64:
                case SpecialType.System_Decimal:
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                    return TypeScriptType.Number;

                case SpecialType.System_String:
                    return TypeScriptType.String;

                case SpecialType.System_IntPtr:
                case SpecialType.System_UIntPtr:
                    return TypeScriptType.Any;

                case SpecialType.System_Array:
                case SpecialType.System_Collections_IEnumerable:
                case SpecialType.System_Collections_Generic_IEnumerable_T:
                case SpecialType.System_Collections_Generic_IList_T:
                case SpecialType.System_Collections_Generic_ICollection_T:
                case SpecialType.System_Collections_IEnumerator:
                case SpecialType.System_Collections_Generic_IEnumerator_T:
                case SpecialType.System_Collections_Generic_IReadOnlyList_T:
                case SpecialType.System_Collections_Generic_IReadOnlyCollection_T:
                    return TypeScriptType.Array;

                case SpecialType.System_Nullable_T:
                    return TypeScriptType.Any;

                case SpecialType.System_DateTime:
                    return TypeScriptType.Date;

                case SpecialType.System_Runtime_CompilerServices_IsVolatile:
                case SpecialType.System_IDisposable:
                case SpecialType.System_TypedReference:
                case SpecialType.System_ArgIterator:
                case SpecialType.System_RuntimeArgumentHandle:
                case SpecialType.System_RuntimeFieldHandle:
                case SpecialType.System_RuntimeMethodHandle:
                case SpecialType.System_RuntimeTypeHandle:
                case SpecialType.System_IAsyncResult:
                case SpecialType.System_AsyncCallback:
                    return TypeScriptType.Any;

                default:
                    return TypeScriptType.Any;
            }
        }

        public static string ConvertTypeScriptTypeToLiteral(TypeScriptType type)
        {
            switch (type)
            {
                case TypeScriptType.Any:
                    return "any";
                case TypeScriptType.Boolean:
                    return "boolean";
                case TypeScriptType.Number:
                    return "number";
                case TypeScriptType.String:
                    return "string";
                case TypeScriptType.Array:
                    return "[]";
                case TypeScriptType.Tuple:
                    return "[]";
                case TypeScriptType.Enum:
                    return "enum";
                case TypeScriptType.Void:
                    return "void";
                case TypeScriptType.Null:
                    return "null";
                case TypeScriptType.Undefined:
                    return "undefined";
                case TypeScriptType.Never:
                    return "never";
                case TypeScriptType.Date:
                    return "Date";
                default:
                    return "any";
            }
        }
    }
}
