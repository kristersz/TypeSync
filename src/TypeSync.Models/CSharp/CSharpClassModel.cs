using System.Collections.Generic;

namespace TypeSync.Models.CSharp
{
    public class CSharpClassModel
    {
        public CSharpClassModel()
        {
            TypeParameters = new List<CSharpTypeParameterModel>();
            Dependencies = new List<CSharpDependencyModel>();
            Properties = new List<CSharpPropertyModel>();
        }

        public string Name { get; set; }

        public string BaseClass { get; set; }

        public bool IsGeneric { get; set; }

        public List<CSharpTypeParameterModel> TypeParameters { get; set; }

        public List<CSharpDependencyModel> Dependencies { get; set; }

        public List<CSharpPropertyModel> Properties { get; set; }
    }
}
