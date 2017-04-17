using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TypeSync.Core.Helpers;
using TypeSync.Core.Models;
using TypeSync.Models.CSharp;

namespace TypeSync.Core.Features.WebApiAnalysis
{
    public class WebApiAnalyzer
    {
        private readonly WebApiAnalysisContext _context;

        public WebApiAnalyzer()
        {
            _context = new WebApiAnalysisContext();
        }

        public AnalysisResult<List<CSharpControllerModel>> Analyze(string path)
        {
            var models = new List<CSharpControllerModel>();

            _context.Init(path);

            var documents = _context.Project.Documents
                .Where(d => d.Name.Contains("Controller"))
                .ToList();

            var syntaxTrees = documents
                .Select(d => d.GetSyntaxTreeAsync().Result)
                .ToList();

            foreach (var syntaxTree in syntaxTrees)
            {
                var root = syntaxTree.GetRoot();
                var semanticModel = _context.Compilation.GetSemanticModel(syntaxTree);

                var classNodes = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();

                foreach (var classNode in classNodes)
                {
                    var classSymbol = semanticModel.GetDeclaredSymbol(classNode) as INamedTypeSymbol;

                    var controllerModel = new CSharpControllerModel();

                    controllerModel.Name = ControllerHelper.ExtractControllerName(classSymbol.Name);

                    var classAttributes = classSymbol.GetAttributes();

                    if (!classAttributes.IsDefaultOrEmpty)
                    {
                        var routePrefix = AttributeHelper.GetRoutePrefix(classAttributes);

                        if (!string.IsNullOrEmpty(routePrefix))
                        {
                            controllerModel.RoutePrefix = routePrefix;
                        }
                    }

                    var methodNodes = classNode.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();

                    foreach (var methodNode in methodNodes)
                    {
                        var methodSymbol = semanticModel.GetDeclaredSymbol(methodNode) as IMethodSymbol;

                        // only public methods can be exposed via HTTP
                        if (methodSymbol.DeclaredAccessibility != Accessibility.Public)
                        {
                            continue;
                        }

                        var methodModel = new CSharpControllerMethodModel();

                        methodModel.Name = methodSymbol.Name;

                        var methodAttributes = methodSymbol.GetAttributes();

                        if (!methodAttributes.IsDefaultOrEmpty)
                        {
                            var httpMethod = AttributeHelper.DetermineHttpMehod(methodAttributes);

                            if (httpMethod.HasValue)
                            {
                                methodModel.HttpMethod = httpMethod.Value;
                            }

                            var route = AttributeHelper.GetRoute(methodAttributes);

                            if (!string.IsNullOrEmpty(route))
                            {
                                methodModel.Route = route;
                            }
                        }

                        controllerModel.Methods.Add(methodModel);
                    }

                    models.Add(controllerModel);
                }
            }

            return new AnalysisResult<List<CSharpControllerModel>>()
            {
                Value = models,
                Success = true
            };
        }
    }
}
