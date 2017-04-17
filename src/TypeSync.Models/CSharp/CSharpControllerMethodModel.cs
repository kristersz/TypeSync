using System;
using System.Collections.Generic;
using TypeSync.Models.Common;

namespace TypeSync.Models.CSharp
{
    public class CSharpControllerMethodModel
    {
        public string Name { get; set; }

        public HttpMethod HttpMethod { get; set; }

        public string Route { get; set; }

        public CSharpTypeModel ReturnType { get; set; }

        public List<Tuple<CSharpTypeModel, string>> Arguments { get; set; }
    }
}
