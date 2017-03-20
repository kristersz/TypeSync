using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using TypeSync.SyntaxWalkers;

namespace TypeSync
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var viewModelText = @"..\..\Source\ViewModels.cs";
                var viewModelPath = File.ReadAllText(viewModelText);

                var tree = CSharpSyntaxTree.ParseText(viewModelPath).WithFilePath(viewModelText);

                var errors = tree.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error);

                if (errors.Any())
                {
                    Console.WriteLine("Syntax contains errors: ");

                    foreach (var error in errors)
                    {
                        Console.WriteLine(error.ToString());
                    }
                }

                var classWalker = new ClassWalker();             

                var root = tree.GetRoot();

                classWalker.Visit(root);

                var classNodes = classWalker.Classes;

                foreach (var classNode in classNodes)
                {
                    var className = classNode.Identifier.Text;

                    Console.WriteLine("Discovered class [{0}]", className);

                    var propertyWalker = new PropertyWalker();

                    propertyWalker.Visit(classNode);

                    var properties = propertyWalker.Properties;

                    Console.WriteLine("With properties:");

                    properties.ForEach(p => Console.WriteLine("[name: {0}; type: {1}]", p.Name, p.Type));

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
