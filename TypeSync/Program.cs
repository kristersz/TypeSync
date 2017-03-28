using System;
using log4net;
using log4net.Config;
using Microsoft.CodeAnalysis.MSBuild;
using TypeSync.Core;
using TypeSync.Core.Analyzers;

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
                var solutionPath = @"C:\Dev\VS2017\TypeSync\Samples\Samples.sln";

                var workspace = MSBuildWorkspace.Create();
                var solution = workspace.OpenSolutionAsync(solutionPath).Result;

                foreach (var project in solution.Projects)
                {
                    Console.WriteLine(project.Name);

                    var compilation = project.GetCompilationAsync().Result;
                    var syntaxTrees = compilation.SyntaxTrees;

                    foreach (var syntaxTree in syntaxTrees)
                    {
                        if (syntaxTree.ToString().Contains("auto-generated"))
                        {
                            continue;
                        }

                        var semanticModel = compilation.GetSemanticModel(syntaxTree);

                        // generate the TypeScript code and output to file
                        var outputter = new TypeScriptOutputter();
                        var generator = new TypeScriptGenerator();
                        var analyzer = new DTOAnalyzer(syntaxTree, semanticModel);

                        var classModels = analyzer.AnalyzeDTOs();

                        foreach (var classModel in classModels)
                        {
                            log.DebugFormat("Class {0}", classModel.Name);

                            var contents = generator.Generate(classModel);

                            log.Debug("Contents generated");

                            outputter.Output(@"C:\Dev\temp\", classModel.Name, contents);

                            log.Debug("Contents outputted");
                        }
                    }

                    foreach (var document in project.Documents)
                    {
                        Console.WriteLine(document.Name);
                    }
                }

                //// parse the syntax tree from a .cs file
                //var viewModelPath = @"..\..\Source\ViewModels.cs";
                //var viewModelText = File.ReadAllText(viewModelPath);

                //var tree = CSharpSyntaxTree.ParseText(viewModelText).WithFilePath(viewModelPath);

                //// check for any syntax errors
                //var errors = tree.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error);

                //if (errors.Any())
                //{
                //    log.Warn("Syntax contains errors: ");

                //    foreach (var error in errors)
                //    {
                //        log.Warn(error.ToString());
                //    }
                //}

                //var root = tree.GetRoot();

                //var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
                //var compilation = CSharpCompilation.Create("MyCompilation", syntaxTrees: new[] { tree }, references: new[] { mscorlib });

                //// note that we must specify the tree for which we want the model.
                //// each tree has its own semantic model
                //var semanticModel = compilation.GetSemanticModel(tree);

                //var classSyntax = tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().First();
                //var classSymbol = semanticModel.GetDeclaredSymbol(classSyntax) as INamedTypeSymbol;

                //// rewrite System types with aliases
                //var aliasRewriter = new PropertyTypeAliasRewriter();
                //var rewriteResult = aliasRewriter.Visit(root);

                //if (root != rewriteResult)
                //{
                //    root = rewriteResult;

                //    File.WriteAllText(viewModelPath, root.ToFullString());
                //    log.Debug("Some property types were replaced with aliases");
                //}

                //// generate the TypeScript code and output to file
                //var outputter = new TypeScriptOutputter();
                //var generator = new TypeScriptGenerator();
                //var analyzer = new DTOAnalyzer(tree, semanticModel);

                //var classModels = analyzer.AnalyzeDTOs();

                //foreach (var classModel in classModels)
                //{
                //    log.DebugFormat("Class {0}", classModel.Name);

                //    var contents = generator.Generate(classModel);

                //    log.Debug("Contents generated");

                //    outputter.Output(@"C:\Dev\temp\", classModel.Name, contents);

                //    log.Debug("Contents outputted");
                //}
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Exception thrown: " + ex.Message);
            }

            log.Info("Exiting TypeSync");
        }     
    }
}
