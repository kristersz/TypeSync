using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using TypeSync.Core.Features.Common;
using TypeSync.Models.CSharp;
using TypeSync.Tests.Infrastructure.Helpers;
using TypeSync.Tests.Infrastructure.TestDoubles;

namespace TypeSync.Tests.Core.Analysis
{
    [TestFixture]
    [Category("TypeAnalysis")]
    public class TypeAnalyzerTest
    {
        [Test]
        public void TypeAnalysis_CLRType_Int64()
        {
            // create syntax tree
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
public class Test
{
    public long Id { get; set; }
}");

            // create compilation
            var compilation = RoslynTestHelpers.CreateTestCompilation(new[] { syntaxTree });

            // extract symbol
            var typeSymbol = RoslynTestHelpers.ExtractPropertyType(compilation, syntaxTree);

            // analyze
            var context = new TestAnalysisContext(compilation);
            var analyzer = new TypeAnalyzer(context);

            var csTypeModel = analyzer.AnalyzePropertyType(typeSymbol);

            // assert
            var expected = new CSharpTypeModel()
            {
                Name = "Int64",
                TypeKind = CSharpTypeKind.Struct,
                SpecialType = CSharpSpecialType.System_Int64
            };

            Assert.AreEqual(expected.Name, csTypeModel.Name);
            Assert.AreEqual(expected.TypeKind, csTypeModel.TypeKind);
            Assert.AreEqual(expected.SpecialType, csTypeModel.SpecialType);
        }
    }
}
