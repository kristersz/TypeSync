using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using TypeSync.Core.Features.ModelAnalysis;
using TypeSync.Models.Converters;
using TypeSync.Output.Generators;
using TypeSync.Test.TestDoubles;

namespace TypeSync.Test.Integration
{
    [TestFixture]
    [Category("ModelGeneration")]
    public class ModelGenerationTest
    {
        [SetUp]
        protected void SetUp()
        {
            // create workspace
        }
 
        [Test]
        public void ModelGeneration_Class_CLRTypes()
        {
            // create syntax tree
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
public class Person
{
    public long Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}");

            // create compilation
            var compilation = CreateTestCompilation(new[] { syntaxTree });

            // analyze
            var root = syntaxTree.GetRoot();
            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            var classDeclaration = root.DescendantNodes().OfType<ClassDeclarationSyntax>().First();
            var classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration);

            var context = new TestAnalysisContext(compilation);
            var analyzer = new ModelAnalyzer(context);

            var csClassModel = analyzer.AnalyzeClassSymbol(classSymbol);

            // convert
            var converter = new ModelConverter();

            var tsClassModel = converter.ConvertClass(csClassModel);

            // generate
            var generator = new ModelGenerator();

            var generated = generator.GenerateClass(tsClassModel, false);

            // assert
            var expected = @"export class Person {
	id: number;
	firstName: string;
	lastName: string;
}
";

            Assert.AreEqual(expected, generated);
        }

        [Test]
        public void ModelGeneration_Class_DateTime()
        {
            // create syntax tree
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
public class Person
{
    public long Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public System.DateTime DateOfBirth { get; set; }
}");

            // create compilation
            var compilation = CreateTestCompilation(new[] { syntaxTree });

            // analyze
            var root = syntaxTree.GetRoot();
            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            var classDeclaration = root.DescendantNodes().OfType<ClassDeclarationSyntax>().First();
            var classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration);

            var context = new TestAnalysisContext(compilation);
            var analyzer = new ModelAnalyzer(context);

            var csClassModel = analyzer.AnalyzeClassSymbol(classSymbol);

            // convert
            var converter = new ModelConverter();

            var tsClassModel = converter.ConvertClass(csClassModel);

            // generate
            var generator = new ModelGenerator();

            var generated = generator.GenerateClass(tsClassModel, false);

            // assert
            var expected = @"export class Person {
	id: number;
	firstName: string;
	lastName: string;
	dateOfBirth: Date;
}
";

            Assert.AreEqual(expected, generated);
        }

        [Test]
        public void ModelGeneration_Class_Dependency()
        {
            // create syntax tree
            var personSyntaxTree = CSharpSyntaxTree.ParseText(@"
using System;
public class Person
{
    public long Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Address Address { get; set; }
}");

            var addressSyntaxTree = CSharpSyntaxTree.ParseText(@"public class Address
{
    public int Id { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public string Street { get; set; }
    public string HouseNumber { get; set; }
}");

            // create compilation
            var compilation = CreateTestCompilation(new[] { personSyntaxTree, addressSyntaxTree });

            // analyze
            var root = personSyntaxTree.GetRoot();
            var semanticModel = compilation.GetSemanticModel(personSyntaxTree);

            var classDeclaration = root.DescendantNodes().OfType<ClassDeclarationSyntax>().First();
            var classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration);

            var context = new TestAnalysisContext(compilation);
            var analyzer = new ModelAnalyzer(context);

            var csClassModel = analyzer.AnalyzeClassSymbol(classSymbol);

            // convert
            var converter = new ModelConverter();

            var tsClassModel = converter.ConvertClass(csClassModel);

            // generate
            var generator = new ModelGenerator();

            var generated = generator.GenerateClass(tsClassModel, false);

            // assert
            var expected = @"import { Address } from './address.model';

export class Person {
	id: number;
	firstName: string;
	lastName: string;
	dateOfBirth: Date;
	address: Address;
}
";

            Assert.AreEqual(expected, generated);
        }


        [Test]
        public void ModelGeneration_Enum_ConstantValues()
        {
            // create syntax tree
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
public enum Gender
{
    None = 0,
    Male = 1,
    Female = 2
}");

            // create compilation
            var compilation = CreateTestCompilation(new[] { syntaxTree });

            // analyze
            var root = syntaxTree.GetRoot();
            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            var enumDeclaration = root.DescendantNodes().OfType<EnumDeclarationSyntax>().First();
            var enumSymbol = semanticModel.GetDeclaredSymbol(enumDeclaration);

            var context = new TestAnalysisContext(compilation);
            var analyzer = new ModelAnalyzer(context);

            var csEnumModel = analyzer.AnalyzeEnumSymbol(enumSymbol);

            // convert
            var converter = new ModelConverter();

            var tsEnumModel = converter.ConvertEnum(csEnumModel);

            // generate
            var generator = new ModelGenerator();

            var generated = generator.GenerateEnum(tsEnumModel, false);

            // assert
            var expected = @"export enum Gender {
	None = 0,
	Male = 1,
	Female = 2
}
";

            Assert.AreEqual(expected, generated);
        }


        private Compilation CreateTestCompilation(IEnumerable<SyntaxTree> syntaxTrees)
        {
            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);

            var compilation = CSharpCompilation.Create(
                "TestCompilation",
                syntaxTrees: syntaxTrees,
                references: new[] { mscorlib }
            );

            return compilation;
        }
    }
}
