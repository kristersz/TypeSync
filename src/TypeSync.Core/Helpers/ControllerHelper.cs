using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TypeSync.Core.Helpers
{
    public static class ControllerHelper
    {
        public static string ExtractControllerName(string controllerFullName)
        {
            if (!string.IsNullOrWhiteSpace(controllerFullName))
            {
                int charLocation = controllerFullName.IndexOf("Controller", StringComparison.Ordinal);

                if (charLocation > 0)
                {
                    return controllerFullName.Substring(0, charLocation);
                }
            }

            return string.Empty;
        }

        public static List<INamedTypeSymbol> GetControllers(Project project, Compilation compilation)
        {
            var controllers = new List<INamedTypeSymbol>();

            var documents = project.Documents
                .Where(d => d.Name.Contains("Controller"))
                .ToList();

            var syntaxTrees = documents
                .Select(d => d.GetSyntaxTreeAsync().Result)
                .ToList();

            foreach (var syntaxTree in syntaxTrees)
            {
                var root = syntaxTree.GetRoot();
                var semanticModel = compilation.GetSemanticModel(syntaxTree);

                var classNodes = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();

                foreach (var classNode in classNodes)
                {
                    var classSymbol = semanticModel.GetDeclaredSymbol(classNode) as INamedTypeSymbol;

                    if (classSymbol.AllInterfaces.Any(i => i.Name == "IHttpController"))
                    {
                        controllers.Add(classSymbol);
                    }
                }
            }

            return controllers;
        }

        public static List<IMethodSymbol> GetPublicMethods(INamedTypeSymbol classSymbol)
        {
            var methodSymbols = classSymbol
                .GetMembers()
                .Where(m => m.Kind == SymbolKind.Method)
                .Cast<IMethodSymbol>()
                .ToList();

            var publicMethods = methodSymbols
                .Where(m => m.DeclaredAccessibility == Accessibility.Public && m.MethodKind == MethodKind.Ordinary)
                .ToList();

            return publicMethods;
        }
    }
}
