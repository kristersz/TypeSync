using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
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

                // rewrite System types with aliases
                var aliasRewriter = new PropertyTypeAliasRewriter();
                var rewriteResult = aliasRewriter.Visit(root);

                if (root != rewriteResult)
                {
                    root = rewriteResult;

                    File.WriteAllText(viewModelPath, root.ToFullString());
                    Console.WriteLine("Some property types were replaced with aliases");
                }

                var classCollector = new ClassCollector();              

                classCollector.Visit(root);

                var classNodes = classCollector.Classes;

                // process each class declaration in the syntax tree
                foreach (var classNode in classNodes)
                {
                    var className = classNode.Identifier.Text;

                    Console.WriteLine("Discovered class [{0}]", className);

                    var propertyCollector = new PropertyCollector();

                    // collect the properties
                    propertyCollector.Visit(classNode);

                    var properties = propertyCollector.Properties;

                    Console.WriteLine("With properties:");

                    properties.ForEach(p => Console.WriteLine("[name: {0}; type: {1}]", p.Name, p.Type));

                    // generate the TypeScript code and output to file
                    var generator = new TypeScriptGenerator();
                    var outputter = new TypeScriptOutputter();

                    var contents = generator.Generate(className, properties);

                    Console.WriteLine("Contents generated");

                    outputter.Output(@"C:\Dev\temp\", className, contents);

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
