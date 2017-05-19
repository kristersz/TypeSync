using System.Collections.Generic;
using System.Linq;
using DataStructures.Graphs;
using Microsoft.CodeAnalysis;
using TypeSync.Core.Features.Common;
using TypeSync.Core.Features.ModelAnalysis;
using TypeSync.Core.Helpers;
using TypeSync.Core.Models;
using TypeSync.Core.Services;
using TypeSync.Models.CSharp;

namespace TypeSync.Core.Features.Synchronization
{
    public class Synchronizer
    {
        private readonly SynchronizationContext _context;
        private readonly TypeAnalyzer _typeAnalyzer;

        public Synchronizer()
        {
            _context = new SynchronizationContext();
            _typeAnalyzer = new TypeAnalyzer(_context);
        }

        public AnalysisResult<CSharpModels> Synchronize(string path)
        {
            _context.Init(path);

            var typesForGeneration = new List<ITypeSymbol>();

            // get all controller classes
            var controllers = ControllerHelper.GetControllers(_context.Project, _context.Compilation);

            // process controllers
            foreach (var controller in controllers)
            {
                // get all controller methods that are exposed via HTTP
                var httpMethods = ControllerHelper.GetPublicMethods(controller);

                foreach (var httpMethod in httpMethods)
                {
                    // collect unique return types
                    if (!typesForGeneration.Exists(t => t.Equals(httpMethod.ReturnType)))
                    {
                        typesForGeneration.Add(httpMethod.ReturnType);
                    }

                    // collect unique parameter types
                    foreach (var parameter in httpMethod.Parameters)
                    {
                        if (!typesForGeneration.Exists(t => t.Equals(parameter.Type)))
                        {
                            typesForGeneration.Add(parameter.Type);
                        }
                    }
                }
            }

            var namedTypes = new List<INamedTypeSymbol>();

            // process collected types
            foreach (var type in typesForGeneration)
            {
                var namedType = type as INamedTypeSymbol;

                if (!namedTypes.Exists(t => t.Equals(namedType)) && TypeHelper.IsSupportedType(type))
                {
                    namedTypes.Add(namedType);
                }

                // handle type arguments
                if (!namedType.TypeArguments.IsDefaultOrEmpty)
                {
                    // TODO: get type members recursively
                    foreach (var typeArgument in namedType.TypeArguments)
                    {
                        var namedArgument = typeArgument as INamedTypeSymbol;

                        if (!namedTypes.Exists(t => t.Equals(namedArgument)) && TypeHelper.IsSupportedType(namedArgument))
                        {
                            namedTypes.Add(namedArgument);
                        }
                    }
                }
            }

            var depGraphs = new List<DirectedSparseGraph<DependencyNode>>();

            foreach (var namedType in namedTypes)
            {
                var depGraphFactory = new DependencyGraphService();

                var graph = depGraphFactory.BuildForNamedTypeSymbol(namedType);

                var readable = graph.ToReadable();

                depGraphs.Add(graph);
            }

            var nodes = depGraphs.SelectMany(g => g.Vertices).Distinct().ToList();

            var modelAnalyzer = new ModelAnalyzer(_context);

            var classModels = new List<CSharpClassModel>();
            var enumModels = new List<CSharpEnumModel>();

            foreach (var node in nodes)
            {
                var symbol = node.NamedTypeSymbol;

                if (symbol.TypeKind == TypeKind.Class || symbol.TypeKind == TypeKind.Struct)
                {
                    classModels.Add(modelAnalyzer.AnalyzeClassSymbol(symbol));
                }
                else if (symbol.TypeKind == TypeKind.Enum)
                {
                    enumModels.Add(modelAnalyzer.AnalyzeEnumSymbol(symbol));
                }
            }

            return new AnalysisResult<CSharpModels>()
            {
                Success = true,
                Value = new CSharpModels()
                {
                    Classes = classModels,
                    Enums = enumModels
                }
            };
        }
    }
}
