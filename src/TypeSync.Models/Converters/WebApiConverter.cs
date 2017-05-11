using System;
using System.Collections.Generic;
using System.Linq;
using TypeSync.Models.CSharp;
using TypeSync.Models.TypeScript;

namespace TypeSync.Models.Converters
{
    public class WebApiConverter
    {
        private readonly TypeConverter _typeConverter;

        public WebApiConverter()
        {
            _typeConverter = new TypeConverter();
        }

        public List<TypeScriptServiceModel> ConvertControllers(List<CSharpControllerModel> controllerModels)
        {
            return controllerModels.Select(c => new TypeScriptServiceModel()
            {
                Name = c.Name,
                RoutePrefix = c.RoutePrefix,
                Imports = c.Dependencies.Select(d => new TypeScriptImportModel()
                {
                    Name = d.Name,
                    FilePath = "",
                    DependencyKind = d.DependencyKind
                }).ToList(),
                Methods = ConvertMethods(c.Methods)
            }).ToList();
        }

        public List<TypeScriptServiceMethodModel> ConvertMethods(List<CSharpControllerMethodModel> methodModels)
        {
            return methodModels.Select(m => new TypeScriptServiceMethodModel()
            {
                Name = m.Name,
                HttpMethod = m.HttpMethod,
                Route = m.Route,
                ReturnType = _typeConverter.ConvertType(m.ReturnType),
                Parameters = m.Parameters
                    .Select(p => new Tuple<TypeScriptTypeModel, string>(_typeConverter.ConvertType(p.Item1), p.Item2))
                    .ToList()
                
            }).ToList();
        }
    }
}
