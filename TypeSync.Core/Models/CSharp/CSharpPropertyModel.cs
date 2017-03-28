using Microsoft.CodeAnalysis;

namespace TypeSync.Core.Models.CSharp
{
    public class CSharpPropertyModel
    {
        public string Name { get; set; }

        public SpecialType SpecialType { get; set; }

        public TypeKind TypeKind { get; set; }

        public SpecialType ElementType { get; set; }
    }
}
