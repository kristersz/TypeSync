using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public void ModelGeneration_Class_BasicClass()
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
            var context = new TestAnalysisContext(compilation);
            var analyzer = new ModelAnalyzer(context);

            var csClassModel = analyzer.AnalyzeClassSymbol(GetClassSymbol(compilation, syntaxTree));

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
            var context = new TestAnalysisContext(compilation);
            var analyzer = new ModelAnalyzer(context);

            var csClassModel = analyzer.AnalyzeClassSymbol(GetClassSymbol(compilation, syntaxTree));

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
            var context = new TestAnalysisContext(compilation);
            var analyzer = new ModelAnalyzer(context);

            var csClassModel = analyzer.AnalyzeClassSymbol(GetClassSymbol(compilation, personSyntaxTree));

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
        public void ModelGeneration_Class_BaseClass()
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
}");

            var studentSyntaxTree = CSharpSyntaxTree.ParseText(@"
using System;
public class Student : Person
{
    public DateTime? YearOfGraduation { get; set; }
}");

            // create compilation
            var compilation = CreateTestCompilation(new[] { personSyntaxTree, studentSyntaxTree });

            // analyze
            var context = new TestAnalysisContext(compilation);
            var analyzer = new ModelAnalyzer(context);

            var csClassModel = analyzer.AnalyzeClassSymbol(GetClassSymbol(compilation, studentSyntaxTree));

            // convert
            var converter = new ModelConverter();
            var tsClassModel = converter.ConvertClass(csClassModel);

            // generate
            var generator = new ModelGenerator();
            var generated = generator.GenerateClass(tsClassModel, false);

            // assert
            var expected = @"import { Person } from './person.model';

export class Student extends Person {
	yearOfGraduation?: Date;
}
";

            Assert.AreEqual(expected, generated);
        }

        [Test]
        public void ModelGeneration_Class_GenericClass()
        {
            // create syntax tree
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
using System.Collections.Generic;
public class PagedDataResponse<T>
{
    public List<T> Rows { get; set; }
    public long TotalCount { get; set; }
}");

            // create compilation
            var compilation = CreateTestCompilation(new[] { syntaxTree });

            // analyze
            var context = new TestAnalysisContext(compilation);
            var analyzer = new ModelAnalyzer(context);

            var csClassModel = analyzer.AnalyzeClassSymbol(GetClassSymbol(compilation, syntaxTree));

            // convert
            var converter = new ModelConverter();
            var tsClassModel = converter.ConvertClass(csClassModel);

            // generate
            var generator = new ModelGenerator();
            var generated = generator.GenerateClass(tsClassModel, false);

            // assert
            var expected = @"export class PagedDataResponse<T> {
	rows: T[];
	totalCount: number;
}
";

            Assert.AreEqual(expected, generated);
        }

        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/ya5y69ds.aspx
        /// </summary>
        [Test]
        public void ModelGeneration_Class_BuiltInTypes()
        {
            // create syntax tree
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
public class AllBuiltInTypes
{
    public bool Truth { get; set; }   
    public byte Bite { get; set; }
    public sbyte SmallBite { get; set; }
    public char Charred { get; set; }
    public decimal Decimus { get; set; }
    public double Twix { get; set; }
    public float Jinn { get; set; }
    public int Intelect { get; set; }
    public uint UnlimitedIntelect { get; set; }
    public long Earthworm { get; set; }
    public ulong Snake { get; set; }
    public object Everyone { get; set; }
    public short Shortayy { get; set; }
    public ushort VShortayy { get; set; }
    public string LoremIpsum { get; set; }
}");

            // create compilation
            var compilation = CreateTestCompilation(new[] { syntaxTree });

            // analyze
            var context = new TestAnalysisContext(compilation);
            var analyzer = new ModelAnalyzer(context);

            var csClassModel = analyzer.AnalyzeClassSymbol(GetClassSymbol(compilation, syntaxTree));

            // convert
            var converter = new ModelConverter();
            var tsClassModel = converter.ConvertClass(csClassModel);

            // generate
            var generator = new ModelGenerator();
            var generated = generator.GenerateClass(tsClassModel, false);

            // assert
            var fields = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("truth", "boolean"),               
                new KeyValuePair<string, string>("bite", "number"),
                new KeyValuePair<string, string>("smallBite", "number"),
                new KeyValuePair<string, string>("charred", "string"),
                new KeyValuePair<string, string>("decimus", "number"),
                new KeyValuePair<string, string>("twix", "number"),
                new KeyValuePair<string, string>("jinn", "number"),
                new KeyValuePair<string, string>("intelect", "number"),
                new KeyValuePair<string, string>("unlimitedIntelect", "number"),
                new KeyValuePair<string, string>("earthworm", "number"),
                new KeyValuePair<string, string>("snake", "number"),
                new KeyValuePair<string, string>("everyone", "any"),
                new KeyValuePair<string, string>("shortayy", "number"),
                new KeyValuePair<string, string>("vShortayy", "number"),
                new KeyValuePair<string, string>("loremIpsum", "string")
            };

            var expected = BuildTypeScriptClass("AllBuiltInTypes", fields);

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
            var context = new TestAnalysisContext(compilation);
            var analyzer = new ModelAnalyzer(context);

            var csEnumModel = analyzer.AnalyzeEnumSymbol(GetEnumSymbol(compilation, syntaxTree));

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

        private INamedTypeSymbol GetClassSymbol(Compilation compilation, SyntaxTree syntaxTree)
        {
            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            var classDeclaration = syntaxTree
                .GetRoot()
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .First();

            return semanticModel.GetDeclaredSymbol(classDeclaration);
        }

        private INamedTypeSymbol GetEnumSymbol(Compilation compilation, SyntaxTree syntaxTree)
        {
            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            var enumDeclaration = syntaxTree
                .GetRoot()
                .DescendantNodes()
                .OfType<EnumDeclarationSyntax>()
                .First();

            return semanticModel.GetDeclaredSymbol(enumDeclaration);
        }

        private string BuildTypeScriptClass(string className, List<KeyValuePair<string, string>> fields)
        {
            var sb = new StringBuilder();

            sb.AppendLine("export class " + className + " {");

            foreach (var field in fields)
            {
                sb.AppendLine("\t" + field.Key + ": " + field.Value + ";");
            }

            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
