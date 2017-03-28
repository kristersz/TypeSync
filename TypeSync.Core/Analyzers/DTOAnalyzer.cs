using System.Collections.Generic;
using log4net;
using Microsoft.CodeAnalysis;
using TypeSync.Core.Models.CSharp;
using TypeSync.Core.SyntaxWalkers;

namespace TypeSync.Core.Analyzers
{
    public class DTOAnalyzer
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DTOAnalyzer));

        private SyntaxTree _syntaxTree;
        private SemanticModel _semanticModel;

        public DTOAnalyzer(SyntaxTree syntaxTree, SemanticModel semanticModel)
        {
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
                    property.TypeKind = propertySymbol.Type.TypeKind;

                    if (propertySymbol.Type.SpecialType != SpecialType.None)
                    {
                        property.SpecialType = propertySymbol.Type.SpecialType;
                    }

                    if (propertySymbol.Type.TypeKind == TypeKind.Array)
                    {
                        var arrayTypeSymbol = propertySymbol.Type as IArrayTypeSymbol;

                        property.ElementType = arrayTypeSymbol.ElementType.SpecialType;
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
