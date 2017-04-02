using Microsoft.CodeAnalysis;
using TypeSync.Models.Enums;

namespace TypeSync.Core.Mappers
{
    public static class TypeMapper
    {
        public static CSharpSpecialType MapSpecialType(SpecialType specialType)
        {
            return (CSharpSpecialType)specialType;
        }

        public static CSharpTypeKind MapTypeKind(TypeKind typeKind)
        {
            return (CSharpTypeKind)typeKind;
        }
    }
}
