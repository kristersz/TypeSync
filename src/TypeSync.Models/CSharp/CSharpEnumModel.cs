using System.Collections.Generic;

namespace TypeSync.Models.CSharp
{
    public class CSharpEnumModel
    {
        public CSharpEnumModel()
        {
            Members = new List<CSharpEnumMemberModel>();
        }

        public string Name { get; set; }

        public List<CSharpEnumMemberModel> Members { get; set; }
    }
}
