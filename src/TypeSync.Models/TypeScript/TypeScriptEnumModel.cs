using System.Collections.Generic;

namespace TypeSync.Models.TypeScript
{
    public class TypeScriptEnumModel
    {
        public TypeScriptEnumModel()
        {
            Members = new List<TypeScriptEnumMemberModel>();
        }

        public string Name { get; set; }

        public List<TypeScriptEnumMemberModel> Members { get; set; }
    }
}
