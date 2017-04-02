using System.Collections.Generic;
using log4net;
using Microsoft.CodeAnalysis;
using TypeSync.Core.SyntaxWalkers;
using TypeSync.Models.CSharp;

namespace TypeSync.Core.Analyzers
{
    public class DTOAnalyzer
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DTOAnalyzer));

        private SemanticModel _semanticModel;

        public DTOAnalyzer(SemanticModel semanticModel)
        {
            _semanticModel = semanticModel;
        }

        public List<CSharpClassModel> AnalyzeDTOs()
        {
            log.Info("DTO analysis started");

            var classModels = new List<CSharpClassModel>();

            var root = _semanticModel.SyntaxTree.GetRoot();

            // collect classes
            var classCollector = new ClassCollector();

            classCollector.Visit(root);

            var classNodes = classCollector.Classes;

            // symbol instances for equality checks
            var IEnumerableSymbol = _semanticModel.Compilation.GetTypeByMetadataName("System.Collections.Generic.IEnumerable`1");

            var typeAnalyzer = new TypeAnalyzer();

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
                    property.Type = typeAnalyzer.AnalyzePropertyType(propertySymbol.Type, IEnumerableSymbol);

                    classModel.Properties.Add(property);
                }

                classModels.Add(classModel);
            }

            log.Info("DTO analysis finished");

            return classModels;
        }
    }
}
