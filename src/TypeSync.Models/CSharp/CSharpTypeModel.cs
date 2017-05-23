using System.Collections.Generic;

namespace TypeSync.Models.CSharp
{
    public class CSharpTypeModel
    {
        public CSharpTypeModel()
        {
            TypeArguments = new List<CSharpTypeModel>();
        }

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
