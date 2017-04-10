using System.Collections.Generic;
using System.Linq;
using DataStructures.Graphs;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using TypeSync.Core.Mappers;
using TypeSync.Core.Models;
using TypeSync.Models.CSharp;

namespace TypeSync.Core.Features.ModelAnalysis
{
    public class ModelAnalyzer
    {
        private readonly ModelAnalysisContext _context;

        public ModelAnalyzer()
        {
            _context = new ModelAnalysisContext();
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

                var dependencies = graph.OutgoingEdges(type).Select(e => e.Destination).ToList();

                var semanticModel = type.SemanticModel;
                var syntaxTree = semanticModel.SyntaxTree;

                if (type.TypeKind == TypeKind.Class)
                {
                    var classModel = new CSharpClassModel() { Name = type.Name };

                    if (type.NamedTypeSymbol.BaseType != null)
                    {
                        classModel.BaseClass = type.NamedTypeSymbol.BaseType.Name;
                    }

                    var internalDependencies = dependencies
                        .Where(d => !d.IsExternal)
                        .ToList();

                    foreach (var dep in internalDependencies)
                    {
                        classModel.Dependencies.Add(new CSharpDependencyModel()
                        {
                            Name = dep.Name,
                            Namespace = dep.Namespace
                        });
                    }

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
                    var enumModel = new CSharpEnumModel() { Name = type.Name };

                    var memberNodes = syntaxTree.GetRoot().DescendantNodes().OfType<EnumMemberDeclarationSyntax>().ToList();

                    foreach (var memberNode in memberNodes)
                    {
                        var memberSymbol = semanticModel.GetDeclaredSymbol(memberNode) as IFieldSymbol;

                        var member = new CSharpEnumMemberModel();

                        member.Name = memberSymbol.Name;
                        member.Value = memberSymbol.HasConstantValue ? (int)memberSymbol.ConstantValue : 0;

                        enumModel.Members.Add(member);
                    }

                   models.Enums.Add(enumModel);
                } 
            }

            return new AnalysisResult<CSharpModels>()
            {
                Value = models,
                Success = true
            };
        }

        private DirectedSparseGraph<DependantType> BuildTypeDependencyGraph()
        {
            var graph = new DirectedSparseGraph<DependantType>();

            //var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            //var mscorlibSymbol = compilation.GetAssemblyOrModuleSymbol(mscorlib);

            //var internalTypes = new List<DependantType>();
            //var externalTypes = new List<DependantType>();

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

                    var referencesToClass = SymbolFinder.FindReferencesAsync(classSymbol, _context.Solution).Result;

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
