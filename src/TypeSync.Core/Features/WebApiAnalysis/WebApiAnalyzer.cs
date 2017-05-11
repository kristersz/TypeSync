using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TypeSync.Core.Features.Common;
using TypeSync.Core.Helpers;
using TypeSync.Core.Models;
using TypeSync.Models.Common;
using TypeSync.Models.CSharp;

namespace TypeSync.Core.Features.WebApiAnalysis
{
    public class WebApiAnalyzer
    {
        private readonly WebApiAnalysisContext _context;
        private readonly TypeAnalyzer _typeAnalyzer;

        public WebApiAnalyzer()
        {
            _context = new WebApiAnalysisContext();
            _typeAnalyzer = new TypeAnalyzer(_context);
        }

        public AnalysisResult<List<CSharpControllerModel>> Analyze(string path)
        {
            var models = new List<CSharpControllerModel>();

            _context.Init(path);

            var isDotNetCore = ProjectHelper.IsDotNetCoreProject(_context.Compilation);

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
                        // .net core projects use Route attribute on controllers instead of RoutePrefix
                        var routePrefix = isDotNetCore ? AttributeHelper.GetRoute(classAttributes) : AttributeHelper.GetRoutePrefix(classAttributes);

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

                        methodModel.ReturnType = _typeAnalyzer.AnalyzePropertyType(methodSymbol.ReturnType);

                        var parameters = methodSymbol.Parameters.ToList();

                        foreach (var parameter in parameters)
                        {
                            methodModel.Parameters.Add(new Tuple<CSharpTypeModel, string>(_typeAnalyzer.AnalyzePropertyType(parameter.Type), parameter.Name));
                        }

                        controllerModel.Methods.Add(methodModel);
                    }

                    var dependencies = GetDependencies(classSymbol);

                    foreach (var dependency in dependencies)
                    {
                        controllerModel.Dependencies.Add(new CSharpDependencyModel()
                        {
                            Name = dependency.Name,
                            Namespace = dependency.ContainingNamespace.ToString(),
                            DependencyKind = dependency.TypeKind == TypeKind.Class ? DependencyKind.Model : DependencyKind.Enum
                        });
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

        private List<INamedTypeSymbol> GetDependencies(INamedTypeSymbol classSymbol)
        {
            var dependencies = new List<INamedTypeSymbol>();

            var members = classSymbol.GetMembers().ToList();

            var methods = members
                .Where(m => m.Kind == SymbolKind.Method)
                .Cast<IMethodSymbol>()
                .ToList();

            foreach (var method in methods)
            {
                var returnType = method.ReturnType;

                if (returnType.ContainingAssembly.Name != "mscorlib")
                {
                    dependencies.Add(returnType as INamedTypeSymbol);
                }
            }           

            return dependencies;
        }
    }
}
