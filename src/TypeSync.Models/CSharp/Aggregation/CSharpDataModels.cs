using System.Collections.Generic;

namespace TypeSync.Models.CSharp
{
    public class CSharpDataModels
    {
        public CSharpDataModels()
        {
            Classes = new List<CSharpClassModel>();
            Enums = new List<CSharpEnumModel>();
        }

        public List<CSharpClassModel> Classes { get; set; }

        public List<CSharpEnumModel> Enums { get; set; }
    }
}
