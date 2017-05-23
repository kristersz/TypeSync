using System.Collections.Generic;

namespace TypeSync.Models.TypeScript
{
    public class TypeScriptTypeModel
    {
        public TypeScriptTypeModel()
        {
            TypeArguments = new List<TypeScriptTypeModel>();
        }

        public string Name { get; set; }

        public bool IsNamedType { get; set; }

        public TypeScriptBasicType PredefinedType { get; set; }

        public TypeScriptTypeModel ElementType { get; set; }

        public List<TypeScriptTypeModel> TypeArguments { get; set; }
    }
}
