using System.Collections.Generic;
using System.Linq;
using TypeSync.Models.CSharp;
using TypeSync.Models.TypeScript;

namespace TypeSync.Models.Converters
{
    public class WebApiConverter
    {
        public List<TypeScriptServiceModel> ConvertControllers(List<CSharpControllerModel> controllerModels)
        {
            return controllerModels.Select(c => new TypeScriptServiceModel()
            {
                Name = c.Name,
                RoutePrefix = c.RoutePrefix,
                Functions = ConvertMethods(c.Methods)
            }).ToList();
        }

        public List<TypeScriptServiceFunctionModel> ConvertMethods(List<CSharpControllerMethodModel> methodModels)
        {
            return methodModels.Select(m => new TypeScriptServiceFunctionModel()
            {
                Name = m.Name,
                HttpMethod = m.HttpMethod,
                Route = m.Route
            }).ToList();
        }
    }
}
