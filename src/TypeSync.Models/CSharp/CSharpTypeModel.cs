using System.Collections.Generic;
using TypeSync.Models.Enums;

namespace TypeSync.Models.CSharp
{
    public class CSharpTypeModel
    {
        public string Name { get; set; }

        public bool IsArray { get; set; }

        public bool IsCollection { get; set; }

        public bool IsNullable { get; set; }

        public CSharpSpecialType SpecialType { get; set; }

        public CSharpTypeKind TypeKind { get; set; }

        public CSharpTypeModel ElementType { get; set; }

        public List<CSharpTypeModel> TypeArguments { get; set; }
    }
}
