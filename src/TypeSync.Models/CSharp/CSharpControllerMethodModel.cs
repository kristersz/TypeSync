using System;
using System.Collections.Generic;
using TypeSync.Models.Common;

namespace TypeSync.Models.CSharp
{
    public class CSharpControllerMethodModel
    {
        public CSharpControllerMethodModel()
        {
            Parameters = new List<Tuple<CSharpTypeModel, string>>();
        }

        public string Name { get; set; }

        public HttpMethod HttpMethod { get; set; }

        public string Route { get; set; }

        public CSharpTypeModel ReturnType { get; set; }

        public List<Tuple<CSharpTypeModel, string>> Parameters { get; set; }
    }
}
