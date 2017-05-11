using System.Collections.Generic;

namespace TypeSync.Models.TypeScript
{
    public class TypeScriptServiceModel
    {
        public TypeScriptServiceModel()
        {
            Imports = new List<TypeScriptImportModel>();
            Functions = new List<TypeScriptServiceFunctionModel>();
        }

        public string Name { get; set; }

        public string RoutePrefix { get; set; }

        public List<TypeScriptImportModel> Imports { get; set; }

        public List<TypeScriptServiceFunctionModel> Functions { get; set; }
    }
}
