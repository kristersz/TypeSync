using System.Collections.Generic;
using System.Linq;
using DataStructures.Graphs;
using log4net;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using TypeSync.Core.Mappers;
using TypeSync.Core.Models;
using TypeSync.Models.Common;
using TypeSync.Models.CSharp;

namespace TypeSync.Core.Features.ModelAnalysis
{
    public class ModelAnalyzer
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ModelAnalyzer));

        private readonly IAnalysisContext _context;

        public ModelAnalyzer(IAnalysisContext context)
        {
            _context = context;
        }

        public AnalysisResult<CSharpModels> Analyze(string path)
        {
            var models = new CSharpModels();

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
                        var x = AnalyzeClassSymbol(type.NamedTypeSymbol);

                        var classModel = new CSharpClassModel() { Name = type.Name };

                        HandleInheritance(classModel, type.NamedTypeSymbol);

                        HandleGenerics(classModel, type.NamedTypeSymbol);

                        var internalDependencies = dependencies
                            .Where(d => !d.IsExternal)
                            .Select(d => d.NamedTypeSymbol)
                            .ToList();

                        HandleDependencies(classModel, internalDependencies);

                        var propertyNodes = syntaxTree.GetRoot().DescendantNodes().OfType<PropertyDeclarationSyntax>().ToList();

                        foreach (var propertyNode in propertyNodes)
                        {
                            var propertySymbol = semanticModel.GetDeclaredSymbol(propertyNode) as IPropertySymbol;

                            var property = new CSharpPropertyModel();

                            property.Name = propertySymbol.Name;
                            property.Type = AnalyzePropertyType(propertySymbol.Type);

                            classModel.Properties.Add(property);
                        }

                        models.Classes.Add(classModel);
                    }
                    else if (type.TypeKind == TypeKind.Enum)
                    {
                        models.Enums.Add(AnalyzeEnumSymbol(type.NamedTypeSymbol));
                    }
                }
            }

            return new AnalysisResult<CSharpModels>()
            {
                Value = models,
                Success = true
            };
        }



        public CSharpClassModel AnalyzeClassSymbol(INamedTypeSymbol classSymbol)
        {
            var classModel = new CSharpClassModel() { Name = classSymbol.Name };

            HandleInheritance(classModel, classSymbol);

            HandleGenerics(classModel, classSymbol);

            var members = classSymbol.GetMembers().ToList();

            var dependencies = new List<INamedTypeSymbol>();          

            var properties = members.Where(m => m.Kind == SymbolKind.Property).ToList();

            foreach (var property in properties)
            {
                var propertySymbol = property as IPropertySymbol;

                var propertyModel = new CSharpPropertyModel()
                {
                    Name = propertySymbol.Name,
                    Type = AnalyzePropertyType(propertySymbol.Type)
                };

                classModel.Properties.Add(propertyModel);

                if (propertySymbol.Type.ContainingAssembly.Equals(classSymbol.ContainingAssembly))
                {
                    dependencies.Add(propertySymbol.Type as INamedTypeSymbol);
                }
            }

            HandleDependencies(classModel, dependencies);

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
                var typeParam = classSymbol.TypeParameters[0];

                model.IsGeneric = true;
                model.TypeParameter = new CSharpTypeParameterModel() { Name = typeParam.Name };
            }
        }

        private void HandleDependencies(CSharpClassModel model, List<INamedTypeSymbol> dependencies)
        {
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



        private CSharpTypeModel AnalyzePropertyType(ITypeSymbol typeSymbol)
        {
            var typeModel = new CSharpTypeModel();

            typeModel.Name = typeSymbol.Name;
            typeModel.TypeKind = TypeMapper.MapTypeKind(typeSymbol.TypeKind);

            if (typeSymbol.SpecialType != SpecialType.None)
            {
                // special types
                typeModel.SpecialType = TypeMapper.MapSpecialType(typeSymbol.SpecialType);
            }
            else if (typeModel.TypeKind == CSharpTypeKind.Array)
            {
                // arrays
                var arrayTypeSymbol = typeSymbol as IArrayTypeSymbol;

                if (arrayTypeSymbol != null)
                {
                    typeModel.IsArray = true;
                    typeModel.ElementType = CreateTypeModel(arrayTypeSymbol.ElementType);
                }
            }
            else if (typeSymbol is INamedTypeSymbol)
            {
                var namedTypeSymbol = typeSymbol as INamedTypeSymbol;

                if (namedTypeSymbol.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T)
                {
                    // nullable types
                    typeModel.IsNullable = true;

                    if (!namedTypeSymbol.TypeArguments.IsDefaultOrEmpty)
                    {
                        var typeArgument = namedTypeSymbol.TypeArguments[0];

                        typeModel.TypeArguments = new List<CSharpTypeModel>() { CreateTypeModel(typeArgument) };
                    }
                }

                var IEnumerableTypeSymbol = _context.Compilation.GetTypeByMetadataName("System.Collections.Generic.IEnumerable`1");

                if (namedTypeSymbol != null
                    && (namedTypeSymbol.ConstructedFrom.Equals(IEnumerableTypeSymbol) || namedTypeSymbol.AllInterfaces.Any(i => i.ConstructedFrom.Equals(IEnumerableTypeSymbol))))
                {

                    // enumerable types such as List<T>, IList<T> or IEnumerable<T>
                    typeModel.IsCollection = true;

                    if (!namedTypeSymbol.TypeArguments.IsDefaultOrEmpty)
                    {
                        var typeArgument = namedTypeSymbol.TypeArguments[0];

                        typeModel.TypeArguments = new List<CSharpTypeModel>() { CreateTypeModel(typeArgument) };
                    }
                }
            }
            else if (typeSymbol is ITypeParameterSymbol)
            {
                // generic type parameter
                var typeParameterSymbol = typeSymbol as ITypeParameterSymbol;
            }

            return typeModel;
        }

        private CSharpTypeModel CreateTypeModel(ITypeSymbol typeSymbol)
        {
            return new CSharpTypeModel()
            {
                Name = typeSymbol.Name,
                TypeKind = TypeMapper.MapTypeKind(typeSymbol.TypeKind),
                SpecialType = TypeMapper.MapSpecialType(typeSymbol.SpecialType)
            };
        }
    }
}
