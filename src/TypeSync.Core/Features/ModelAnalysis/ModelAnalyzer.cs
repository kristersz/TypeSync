using System.Collections.Generic;
using System.Linq;
using DataStructures.Graphs;
using log4net;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using TypeSync.Core.Features.Common;
using TypeSync.Core.Models;
using TypeSync.Core.Services;
using TypeSync.Models.Common;
using TypeSync.Models.CSharp;

namespace TypeSync.Core.Features.ModelAnalysis
{
    public class ModelAnalyzer
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ModelAnalyzer));

        private readonly IAnalysisContext _context;
        private readonly TypeAnalyzer _typeAnalyzer;

        public ModelAnalyzer(IAnalysisContext context)
        {
            _context = context;
            _typeAnalyzer = new TypeAnalyzer(_context);
        }

        public AnalysisResult<CSharpDataModels> Analyze(string path)
        {
            var models = new CSharpDataModels();

            _context.Init(path);

            var graph = BuildTypeDependencyGraph();

            var readable = graph.ToReadable();

            var types = graph.Vertices;

            foreach (var type in types)
            {
                if (type.IsExternal)
                {
                    continue;
                }

                var dependencies = graph.OutgoingEdges(type)
                    .Select(e => e.Destination)
                    .Where(d => d.TypeKind == TypeKind.Class || d.TypeKind == TypeKind.Enum)
                    .ToList();

                var semanticModel = type.SemanticModel;

                if (semanticModel == null)
                {
                    if (type.NamedTypeSymbol != null)
                    {
                        if (type.TypeKind == TypeKind.Enum)
                        {
                            models.Enums.Add(AnalyzeEnumSymbol(type.NamedTypeSymbol));
                        }
                    }  
                }
                else
                {
                    var syntaxTree = semanticModel.SyntaxTree;

                    if (type.TypeKind == TypeKind.Class)
                    {
                        //var classModel = new CSharpClassModel() { Name = type.Name };

                        //HandleInheritance(classModel, type.NamedTypeSymbol);

                        //HandleGenerics(classModel, type.NamedTypeSymbol);

                        //var internalDependencies = dependencies
                        //    .Where(d => !d.IsExternal)
                        //    .Select(d => d.NamedTypeSymbol)
                        //    .ToList();

                        //HandleDependencies(classModel, internalDependencies);

                        //var propertyNodes = syntaxTree.GetRoot().DescendantNodes().OfType<PropertyDeclarationSyntax>().ToList();

                        //foreach (var propertyNode in propertyNodes)
                        //{
                        //    var propertySymbol = semanticModel.GetDeclaredSymbol(propertyNode) as IPropertySymbol;

                        //    var property = new CSharpPropertyModel();

                        //    property.Name = propertySymbol.Name;
                        //    property.Type = _typeAnalyzer.AnalyzePropertyType(propertySymbol.Type);

                        //    classModel.Properties.Add(property);
                        //}

                        // models.Classes.Add(classModel);

                        models.Classes.Add(AnalyzeClassSymbol(type.NamedTypeSymbol));
                    }
                    else if (type.TypeKind == TypeKind.Enum)
                    {
                        models.Enums.Add(AnalyzeEnumSymbol(type.NamedTypeSymbol));
                    }
                }
            }

            return new AnalysisResult<CSharpDataModels>()
            {
                Value = models,
                Success = true
            };
        }



        public CSharpClassModel AnalyzeClassSymbol(INamedTypeSymbol classSymbol)
        {
            var classModel = new CSharpClassModel() { Name = classSymbol.Name };

            var members = classSymbol.GetMembers().ToList();

            HandleInheritance(classModel, classSymbol);
            HandleGenerics(classModel, classSymbol);
            HandleDependencies(classModel, classSymbol);

            var properties = members.Where(m => m.Kind == SymbolKind.Property).ToList();

            foreach (var property in properties)
            {
                var propertySymbol = property as IPropertySymbol;

                // for generic types we want to generate the interface instead of the specific, substituted property types
                if (!propertySymbol.OriginalDefinition.Equals(propertySymbol))
                {
                    propertySymbol = propertySymbol.OriginalDefinition;
                }

                var propertyModel = new CSharpPropertyModel()
                {
                    Name = propertySymbol.Name,
                    Type = _typeAnalyzer.AnalyzeType(propertySymbol.Type)
                };

                classModel.Properties.Add(propertyModel);
            }

            return classModel;
        }

        public CSharpEnumModel AnalyzeEnumSymbol(INamedTypeSymbol enumSymbol)
        {
            var enumModel = new CSharpEnumModel() { Name = enumSymbol.Name };

            var fields = enumSymbol.GetMembers()
                .Where(m => m.Kind == SymbolKind.Field)
                .ToList();

            foreach (var field in fields)
            {
                var fieldSymbol = field as IFieldSymbol;

                var member = new CSharpEnumMemberModel()
                {
                    Name = fieldSymbol.Name,
                    Value = fieldSymbol.HasConstantValue ? (int)fieldSymbol.ConstantValue : 0
                };

                enumModel.Members.Add(member);
            }

            return enumModel;
        }



        private DirectedSparseGraph<DependantType> BuildTypeDependencyGraph()
        {
            var graph = new DirectedSparseGraph<DependantType>();

            var processedTypes = new List<DependantType>();

            foreach (var syntaxTree in _context.Compilation.SyntaxTrees)
            {
                var root = syntaxTree.GetRoot();
                var semanticModel = _context.Compilation.GetSemanticModel(syntaxTree);

                var classNodes = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();

                foreach (var classNode in classNodes)
                {
                    DependantType type = null;

                    var classSymbol = semanticModel.GetDeclaredSymbol(classNode) as INamedTypeSymbol;

                    var processedType = processedTypes.FirstOrDefault(t => t.Name == classSymbol.Name);

                    if (processedType != null)
                    {
                        // we already added this to the graph as a dependency earlier
                        type = processedType;

                        type.SemanticModel = semanticModel;
                        type.NamedTypeSymbol = classSymbol;
                    }
                    else
                    {
                        type = new DependantType()
                        {
                            Name = classSymbol.Name,
                            Namespace = classSymbol.ContainingNamespace.ToString(),
                            ContainingAssembly = classSymbol.ContainingAssembly.Name,
                            IsExternal = false,
                            SemanticModel = semanticModel,
                            NamedTypeSymbol = classSymbol,
                            TypeKind = classSymbol.TypeKind
                        };

                        processedTypes.Add(type);
                        graph.AddVertex(type);
                    }

                    if (_context.Solution != null)
                    {
                        var referencesToClass = SymbolFinder.FindReferencesAsync(classSymbol, _context.Solution).Result;
                    }                   

                    var classDependencies = classNode.DescendantNodes()
                        .Select(n => semanticModel.GetTypeInfo(n).Type)
                        .Where(n => n != null)
                        .Distinct()
                        .ToList();

                    foreach (var dependency in classDependencies)
                    {
                        DependantType dep = null;

                        var processedDep = processedTypes.FirstOrDefault(t => t.Name == dependency.Name);

                        if (processedDep != null)
                        {
                            dep = processedDep;
                        }
                        else
                        {
                            dep = new DependantType()
                            {
                                Name = dependency.Name,
                                Namespace = dependency.ContainingNamespace.ToString(),
                                ContainingAssembly = dependency.ContainingAssembly.Name,
                                NamedTypeSymbol = dependency as INamedTypeSymbol,
                                TypeKind = dependency.TypeKind,
                                IsExternal = dependency.ContainingAssembly.Name == "mscorlib"
                            };

                            processedTypes.Add(dep);
                        }

                        if (!graph.HasVertex(dep))
                        {
                            graph.AddVertex(dep);
                        }

                        graph.AddEdge(type, dep);
                    }
                }


                var enumNodes = root.DescendantNodes().OfType<EnumDeclarationSyntax>().ToList();

                foreach (var enumNode in enumNodes)
                {
                    DependantType type = null;

                    var enumSymbol = semanticModel.GetDeclaredSymbol(enumNode) as INamedTypeSymbol;

                    var processedType = processedTypes.FirstOrDefault(t => t.Name == enumSymbol.Name);

                    if (processedType != null)
                    {
                        // we already added this to the graph as a dependency earlier
                        type = processedType;

                        type.SemanticModel = semanticModel;
                        type.NamedTypeSymbol = enumSymbol;
                    }
                    else
                    {
                        type = new DependantType()
                        {
                            Name = enumSymbol.Name,
                            Namespace = enumSymbol.ContainingNamespace.ToString(),
                            ContainingAssembly = enumSymbol.ContainingAssembly.Name,
                            IsExternal = false,
                            SemanticModel = semanticModel,
                            NamedTypeSymbol = enumSymbol,
                            TypeKind = enumSymbol.TypeKind
                        };

                        processedTypes.Add(type);
                        graph.AddVertex(type);
                    }
                }
            }

            return graph;
        }

        private void HandleInheritance(CSharpClassModel model, INamedTypeSymbol classSymbol)
        {
            if (classSymbol.BaseType != null && classSymbol.BaseType.ContainingAssembly.Name != "mscorlib")
            {
                model.BaseClass = classSymbol.BaseType.Name;
            }
        }

        private void HandleGenerics(CSharpClassModel model, INamedTypeSymbol classSymbol)
        {
            if (classSymbol.IsGenericType && !classSymbol.TypeParameters.IsDefaultOrEmpty)
            {
                model.IsGeneric = true;

                foreach (var typeParameter in classSymbol.TypeParameters)
                {
                    model.TypeParameters.Add(new CSharpTypeParameterModel() { Name = typeParameter.Name });
                }               
            }
        }

        private void HandleDependencies(CSharpClassModel model, INamedTypeSymbol classSymbol)
        {
            var depService = new DependencyService();
            var dependencies = depService.GetTypeDependencies(classSymbol);

            foreach (var dep in dependencies)
            {
                model.Dependencies.Add(new CSharpDependencyModel()
                {
                    Name = dep.Name,
                    Namespace = dep.ContainingNamespace.ToString(),
                    DependencyKind = dep.TypeKind == TypeKind.Class ? DependencyKind.Model : DependencyKind.Enum
                });
            }
        }
    }
}
