using System.Collections.Generic;

namespace TypeSync.Models.CSharp
{
    public class CSharpControllerModel
    {
        public CSharpControllerModel()
        {
            Methods = new List<CSharpControllerMethodModel>();
        }

        public string Name { get; set; }

        public string RoutePrefix { get; set; }

        public List<CSharpControllerMethodModel> Methods { get; set; }
    }
}
