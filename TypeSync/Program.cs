using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TypeSync.Models;
using TypeSync.SyntaxRewriters;
using TypeSync.SyntaxWalkers;

namespace TypeSync
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // parse the syntax tree from a .cs file
                var viewModelPath = @"..\..\Source\ViewModels.cs";
                var viewModelText = File.ReadAllText(viewModelPath);

                var tree = CSharpSyntaxTree.ParseText(viewModelText).WithFilePath(viewModelPath);

                // check for any syntax errors
                var errors = tree.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error);

                if (errors.Any())
                {
                    Console.WriteLine("Syntax contains errors: ");

                    foreach (var error in errors)
                    {
                        Console.WriteLine(error.ToString());
                    }
                }

                var root = tree.GetRoot();

                var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
                var compilation = CSharpCompilation.Create("MyCompilation", syntaxTrees: new[] { tree }, references: new[] { mscorlib });             

                // note that we must specify the tree for which we want the model.
                // each tree has its own semantic model
                var semanticModel = compilation.GetSemanticModel(tree);

                var classSyntax = tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().First();
                var classSymbol = semanticModel.GetDeclaredSymbol(classSyntax) as INamedTypeSymbol;

                // rewrite System types with aliases
                var aliasRewriter = new PropertyTypeAliasRewriter();
                var rewriteResult = aliasRewriter.Visit(root);

                if (root != rewriteResult)
                {
                    root = rewriteResult;

                    File.WriteAllText(viewModelPath, root.ToFullString());
                    Console.WriteLine("Some property types were replaced with aliases");
                }

                // generate the TypeScript code and output to file
                var outputter = new TypeScriptOutputter();
                var generator = new TypeScriptGenerator(tree, semanticModel);

                var classModels = generator.AnalyzeDTOs();

                foreach (var classModel in classModels)
                {
                    Console.WriteLine("Class {0}", classModel.Name);

                    var contents = generator.Generate(classModel);

                    Console.WriteLine("Contents generated");

                    outputter.Output(@"C:\Dev\temp\", classModel.Name, contents);

                    Console.WriteLine("Contents outputted");
                    Console.WriteLine();
                }               
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception thrown: " + ex.Message);
            }           
        }     
    }
}
