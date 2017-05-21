using System.Collections.Generic;
using TypeSync.Models.TypeScript;

namespace TypeSync.Models.Angular
{
    public class AngularFormValidatorModel
    {
        public AngularFormValidatorModel()
        {
            Imports = new List<TypeScriptImportModel>();
        }

        public string Name { get; set; }
        
        public List<TypeScriptImportModel> Imports { get; set; }
    }
}
