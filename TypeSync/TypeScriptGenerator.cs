using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using TypeSync.Extensions;
using TypeSync.Models.CSharp;
using TypeSync.SyntaxWalkers;

namespace TypeSync
{
    public class TypeScriptGenerator
    {
        private SyntaxTree _syntaxTree;
        private SemanticModel _semanticModel;

        public TypeScriptGenerator(SyntaxTree syntaxTree, SemanticModel semanticModel)
        {
            _syntaxTree = syntaxTree;
            _semanticModel = semanticModel;
        }

        public List<CSharpClassModel> AnalyzeDTOs()
        {
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

            return classModels;
        }

        public string Generate(CSharpClassModel classModel)
        {
            var sb = new StringBuilder();

            // imports
            sb.AppendLine();

            // class declaration
            sb.AppendLine("export class " + classModel.Name + " {");

            // properties
            foreach (var property in classModel.Properties)
            {
                var tsType = TypeConverter.ConvertCSharpTypeToTypeScript(property.SpecialType);
                var typeLiteral = TypeConverter.ConvertTypeScriptTypeToLiteral(tsType);

                if (property.TypeKind == TypeKind.Array)
                {
                    var elementTsType = TypeConverter.ConvertCSharpTypeToTypeScript(property.ElementType);
                    var elementTypeLiteral = TypeConverter.ConvertTypeScriptTypeToLiteral(elementTsType);

                    typeLiteral = elementTypeLiteral + typeLiteral;
                }

                sb.AppendLine("\t" + property.Name.PascalToCamelCase() + ": " + typeLiteral + ";");
            }

            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
