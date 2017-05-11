using System.Collections.Generic;

namespace TypeSync.Models.CSharp
{
    public class CSharpControllerModel
    {
        public CSharpControllerModel()
        {
            Dependencies = new List<CSharpDependencyModel>();
            Methods = new List<CSharpControllerMethodModel>();
        }

        public string Name { get; set; }

        public string RoutePrefix { get; set; }

        public List<CSharpDependencyModel> Dependencies { get; set; }

        public List<CSharpControllerMethodModel> Methods { get; set; }
    }
}
