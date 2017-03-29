using System.Collections.Generic;
using System.Linq;
using log4net;
using Microsoft.CodeAnalysis;
using TypeSync.Core.Models.CSharp;
using TypeSync.Core.SyntaxWalkers;

namespace TypeSync.Core.Analyzers
{
    public class DTOAnalyzer
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DTOAnalyzer));

        private Compilation _compilation;
        private SyntaxTree _syntaxTree;
        private SemanticModel _semanticModel;

        public DTOAnalyzer(Compilation compilation, SyntaxTree syntaxTree, SemanticModel semanticModel)
        {
            _compilation = compilation;
            _syntaxTree = syntaxTree;
            _semanticModel = semanticModel;
        }

        public List<CSharpClassModel> AnalyzeDTOs()
        {
            log.Info("DTO analysis started");

            var classModels = new List<CSharpClassModel>();

            var root = _syntaxTree.GetRoot();

            // collect classes
            var classCollector = new ClassCollector();

            classCollector.Visit(root);

            var classNodes = classCollector.Classes;

            // symbol instances for equality checks
            var IEnumerableSymbol = _compilation.GetTypeByMetadataName("System.Collections.Generic.IEnumerable`1");

            // process each class declaration in the syntax tree
            foreach (var classNode in classNodes)
            {
                var classModel = new CSharpClassModel() { Name = classNode.Identifier.Text };

                var propertyCollector = new PropertyCollector();

                // collect the properties
                propertyCollector.Visit(classNode);

                var propertySyntaxes = propertyCollector.Properties;

                foreach (var propertySyntax in propertySyntaxes)
                {
                    var propertySymbol = _semanticModel.GetDeclaredSymbol(propertySyntax) as IPropertySymbol;

                    var property = new CSharpPropertyModel();

                    property.Name = propertySymbol.Name;
                    property.Type.Name = propertySymbol.Type.Name;
                    property.Type.TypeKind = propertySymbol.Type.TypeKind;

                    
                    if (propertySymbol.Type.SpecialType != SpecialType.None)
                    {
                        // special types
                        property.Type.SpecialType = propertySymbol.Type.SpecialType;
                    }
                    else if (property.Type.TypeKind == TypeKind.Array)
                    {
                        // arrays
                        var arrayTypeSymbol = propertySymbol.Type as IArrayTypeSymbol;

                        if (arrayTypeSymbol != null)
                        {
                            property.Type.IsCollection = true;
                            property.Type.ElementType = new CSharpTypeModel()
                            {
                                Name = arrayTypeSymbol.ElementType.Name,
                                TypeKind = arrayTypeSymbol.ElementType.TypeKind,
                                SpecialType = arrayTypeSymbol.ElementType.SpecialType
                            };
                        }
                    }                   
                    else if (propertySymbol.Type is INamedTypeSymbol)
                    {                       
                        var namedTypeSymbol = propertySymbol.Type as INamedTypeSymbol;

                        // enumerable types such as List<T>, IList<T> or IEnumerable<T>
                        if (namedTypeSymbol != null && (namedTypeSymbol.ConstructedFrom.Equals(IEnumerableSymbol) || namedTypeSymbol.AllInterfaces.Any(i => i.ConstructedFrom.Equals(IEnumerableSymbol))))
                        {
                            property.Type.IsCollection = true;

                            if (!namedTypeSymbol.TypeArguments.IsDefaultOrEmpty)
                            {
                                var typeArgument = namedTypeSymbol.TypeArguments[0];

                                property.Type.ElementType = new CSharpTypeModel()
                                {
                                    Name = typeArgument.Name,
                                    TypeKind = typeArgument.TypeKind,
                                    SpecialType = typeArgument.SpecialType
                                };
                            }
                        } 
                    }

                    classModel.Properties.Add(property);
                }

                classModels.Add(classModel);
            }

            log.Info("DTO analysis finished");

            return classModels;
        }
    }
}
