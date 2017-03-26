using System;
using System.IO;
using System.Linq;
using log4net;
using log4net.Config;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TypeSync.SyntaxRewriters;

[assembly: XmlConfigurator(Watch = true)]

namespace TypeSync
{
    public class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        public static void Main(string[] args)
        {
            log.Info("Entering TypeSync");

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
                    log.Warn("Syntax contains errors: ");

                    foreach (var error in errors)
                    {
                        log.Warn(error.ToString());
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
                    log.Debug("Some property types were replaced with aliases");
                }

                // generate the TypeScript code and output to file
                var outputter = new TypeScriptOutputter();
                var generator = new TypeScriptGenerator(tree, semanticModel);

                var classModels = generator.AnalyzeDTOs();

                foreach (var classModel in classModels)
                {
                    log.DebugFormat("Class {0}", classModel.Name);

                    var contents = generator.Generate(classModel);

                    log.Debug("Contents generated");

                    outputter.Output(@"C:\Dev\temp\", classModel.Name, contents);

                    log.Debug("Contents outputted");
                }
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Exception thrown: " + ex.Message);
            }

            log.Info("Exiting TypeSync");
        }     
    }
}
