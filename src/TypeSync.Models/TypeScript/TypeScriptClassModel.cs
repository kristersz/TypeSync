using System.Collections.Generic;

namespace TypeSync.Models.TypeScript
{
    public class TypeScriptClassModel
    {
        public TypeScriptClassModel()
        {
            Imports = new List<TypeScriptImportModel>();
            Properties = new List<TypeScriptPropertyModel>();
        }

        public string Name { get; set; }

        public string BaseClass { get; set; }

        public bool IsGeneric { get; set; }

        public TypeScriptTypeParameterModel TypeParameter { get; set; }

        public List<TypeScriptImportModel> Imports { get; set; }

        public List<TypeScriptPropertyModel> Properties { get; set; }
    }
}
