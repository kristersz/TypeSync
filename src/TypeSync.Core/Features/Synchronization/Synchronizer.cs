using System.Collections.Generic;
using System.Linq;
using DataStructures.Graphs;
using Microsoft.CodeAnalysis;
using TypeSync.Core.Factories;
using TypeSync.Core.Features.Common;
using TypeSync.Core.Features.ModelAnalysis;
using TypeSync.Core.Helpers;
using TypeSync.Core.Models;
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

            var classSymbols = ControllerHelper.GetControllers(_context.Project, _context.Compilation);

            foreach (var classSymbol in classSymbols)
            {
                var publicMethodSymbols = ControllerHelper.GetPublicMethods(classSymbol);

                foreach (var methodSymbol in publicMethodSymbols)
                {
                    if (!typesForGeneration.Exists(t => t.Equals(methodSymbol.ReturnType)))
                    {
                        typesForGeneration.Add(methodSymbol.ReturnType);
                    }

                    foreach (var parameter in methodSymbol.Parameters)
                    {
                        if (!typesForGeneration.Exists(t => t.Equals(parameter.Type)))
                        {
                            typesForGeneration.Add(parameter.Type);
                        }
                    }
                }
            }

            var namedTypes = new List<INamedTypeSymbol>();

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
                var depGraphFactory = new DependencyGraphFactory();

                var graph = depGraphFactory.BuildForNamedTypeSymbol(namedType);

                var readable = graph.ToReadable();

                depGraphs.Add(graph);
            }

            var nodes = depGraphs.SelectMany(g => g.Vertices).Distinct();

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
