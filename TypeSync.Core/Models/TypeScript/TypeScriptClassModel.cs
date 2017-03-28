using System.Collections.Generic;

namespace TypeSync.Core.Models.TypeScript
{
    public class TypeScriptClassModel
    {
        public TypeScriptClassModel()
        {
            Properties = new List<TypeScriptPropertyModel>();
        }

        public string Name { get; set; }

        public List<TypeScriptPropertyModel> Properties { get; set; }
    }
}
