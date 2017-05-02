using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TypeSync.Tests.Infrastructure.Helpers
{
    public static class RoslynTestHelpers
    {
        public static Compilation CreateTestCompilation(IEnumerable<SyntaxTree> syntaxTrees)
        {
            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);

            var compilation = CSharpCompilation.Create(
                "TestCompilation",
                syntaxTrees: syntaxTrees,
                references: new[] { mscorlib }
            );

            return compilation;
        }

        public static INamedTypeSymbol GetClassSymbol(Compilation compilation, SyntaxTree syntaxTree)
        {
            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            var classDeclaration = syntaxTree
                .GetRoot()
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .First();

            return semanticModel.GetDeclaredSymbol(classDeclaration);
        }

        public static INamedTypeSymbol GetEnumSymbol(Compilation compilation, SyntaxTree syntaxTree)
        {
            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            var enumDeclaration = syntaxTree
                .GetRoot()
                .DescendantNodes()
                .OfType<EnumDeclarationSyntax>()
                .First();

            return semanticModel.GetDeclaredSymbol(enumDeclaration);
        }

        public static ITypeSymbol ExtractPropertyType(Compilation compilation, SyntaxTree syntaxTree)
        {
            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            var propertyDeclaration = syntaxTree
                .GetRoot()
                .DescendantNodes()
                .OfType<PropertyDeclarationSyntax>()
                .First();

            var symbol = semanticModel.GetDeclaredSymbol(propertyDeclaration);

            return symbol.Type;
        }
    }
}
