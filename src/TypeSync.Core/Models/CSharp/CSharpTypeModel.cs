using Microsoft.CodeAnalysis;

namespace TypeSync.Core.Models.CSharp
{
    public class CSharpTypeModel
    {
        public string Name { get; set; }

        public bool IsCollection { get; set; }

        public SpecialType SpecialType { get; set; }

        public TypeKind TypeKind { get; set; }

        public CSharpTypeModel ElementType { get; set; }
    }
}
