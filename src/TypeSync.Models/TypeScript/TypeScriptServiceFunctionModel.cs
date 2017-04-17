using System;
using System.Collections.Generic;
using TypeSync.Models.Common;

namespace TypeSync.Models.TypeScript
{
    public class TypeScriptServiceFunctionModel
    {
        public string Name { get; set; }

        public HttpMethod HttpMethod { get; set; }

        public string Route { get; set; }

        public TypeScriptTypeModel ReturnType { get; set; }

        public List<Tuple<TypeScriptTypeModel, string>> Arguments { get; set; }
    }
}
