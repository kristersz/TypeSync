using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TypeSync.Core.Helpers;
using TypeSync.Core.Models;
using TypeSync.Models.CSharp;

namespace TypeSync.Core.Features.ValidationAnalysis
{
    public class ValidationAnalyzer
    {
        private readonly ValidationAnalysisContext _context;

        public ValidationAnalyzer()
        {
            _context = new ValidationAnalysisContext();
        }

        public AnalysisResult<List<CSharpValidationAttributeModel>> Analyze(string path)
        {
            var models = new List<CSharpValidationAttributeModel>();

            _context.Init(path);

            foreach (var syntaxTree in _context.Compilation.SyntaxTrees)
            {
                var root = syntaxTree.GetRoot();
                var semanticModel = _context.Compilation.GetSemanticModel(syntaxTree);

                var classNodes = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();

                foreach (var classNode in classNodes)
                {
                    var classSymbol = semanticModel.GetDeclaredSymbol(classNode) as INamedTypeSymbol;
                    var members = classSymbol.GetMembers();

                    var properties = members.Where(m => m.Kind == SymbolKind.Property).ToList();

                    foreach (var property in properties)
                    {
                        var propertySymbol = property as IPropertySymbol;

                        var validationAttributes = AttributeHelper.GetValidationAttributes(propertySymbol);

                        if (validationAttributes.Any())
                        {
                            foreach (var validationAttribute in validationAttributes)
                            {
                                models.Add(new CSharpValidationAttributeModel()
                                {
                                    //Name = validationAttribute.AttributeClass.Name
                                });
                            }
                        }
                    }
                }
            }

            return new AnalysisResult<List<CSharpValidationAttributeModel>>()
            {
                Value = models,
                Success = true
            };
        }
    }
}
