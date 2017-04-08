using System.Collections.Generic;

namespace TypeSync.Models.CSharp
{
    public class CSharpClassModel
    {
        public CSharpClassModel()
        {
            Properties = new List<CSharpPropertyModel>();
        }

        public string Name { get; set; }

        public string BaseClass { get; set; }

        public List<CSharpPropertyModel> Properties { get; set; }
    }
}
