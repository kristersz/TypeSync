using TypeSync.Models.Enums;

namespace TypeSync.Models.TypeScript
{
    public class TypeScriptTypeModel
    {
        public string Name { get; set; }

        public bool IsNamedType { get; set; }

        public TypeScriptBasicType PredefinedType { get; set; }

        public TypeScriptTypeModel ElementType { get; set; }
    }
}
