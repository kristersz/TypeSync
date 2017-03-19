using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TypeSync.Extensions;
using TypeSync.Models;

namespace TypeSync
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var viewModelText = @"..\..\Source\MyViewModel.cs";
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

                var root = tree.GetRoot();

                var classDeclarationSyntax = root.DescendantNodes().OfType<ClassDeclarationSyntax>().First();
                var propertySyntaxes = classDeclarationSyntax.DescendantNodes().OfType<PropertyDeclarationSyntax>().ToList();

                var className = classDeclarationSyntax.Identifier.Text;
                var properties = new List<Property>();

                Console.WriteLine("Class name: " + className);

                foreach (var propSyntax in propertySyntaxes)
                {
                    var newProp = new Property()
                    {
                        Name = propSyntax.Identifier.Text,
                        Type = propSyntax.Type.ToString()
                    };

                    properties.Add(newProp);

                    Console.WriteLine("Property {0} of type {1}", newProp.Name, newProp.Type);
                }

                var sb = new StringBuilder();

                // imports
                sb.AppendLine();

                // class declaration
                sb.AppendLine("export class " + className + " {");

                // properties
                foreach (var property in properties)
                {
                    sb.AppendLine("\t" + property.Name.PascalToCamelCase() + ": " + TypeConverter.ConvertCSharpTypeToTypeScript(property.Type) + ";");
                }

                sb.AppendLine("}");
                sb.AppendLine();

                // write to file
                File.WriteAllText(@"C:\Dev\temp\" + className.PascalToKebabCase() + ".model.ts", sb.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception thrown: " + ex.Message);
            }           
        }     
    }
}
