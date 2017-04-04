using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStructures.Graphs;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.MSBuild;
using TypeSync.Models.CSharp;

namespace TypeSync.Core.Services
{
    public class WorkspaceService
    {
        private readonly MSBuildWorkspace _workspace;

        private Solution _solution;
        private Project _project;

        public MSBuildWorkspace Workspace => _workspace;

        public WorkspaceService()
        {
            _workspace = MSBuildWorkspace.Create();
        }

        public void OpenSolution(string path)
        {
            _solution = _workspace.OpenSolutionAsync(path).Result;
        }

        public void OpenProject(string path)
        {
            _project = _workspace.OpenProjectAsync(path).Result;
            _solution = _project.Solution;
        }

        public ProjectDependencyGraph GetProjectDependencyGraph()
        {
            return _solution.GetProjectDependencyGraph();
        }

        public void GetClassDependencyGraph()
        {
            var graph = new DirectedSparseGraph<DependantType>();

            var compilation = _project.GetCompilationAsync().Result;

            //var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            //var mscorlibSymbol = compilation.GetAssemblyOrModuleSymbol(mscorlib);
            var externalTypes = new List<DependantType>();            

            foreach (var syntaxTree in compilation.SyntaxTrees)
            {
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var classNodes = syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();

                foreach (var classNode in classNodes)
                {
                    var classSymbol = semanticModel.GetDeclaredSymbol(classNode) as INamedTypeSymbol;

                    var dependantType = new DependantType()
                    {
                        Name = classSymbol.ContainingNamespace + "." + classSymbol.Name,
                        ContainingAssembly = classSymbol.ContainingAssembly.Name
                    };

                    graph.AddVertex(dependantType);

                    var referencesToClass = SymbolFinder.FindReferencesAsync(classSymbol, _solution).Result;

                    var classDependencies = classNode.DescendantNodes()
                        .Select(n => semanticModel.GetTypeInfo(n).Type)
                        .Where(n => n != null)
                        .Distinct()
                        .ToList();

                    foreach (var dependency in classDependencies)
                    {
                        var dep = new DependantType()
                        {
                            Name = dependency.ContainingNamespace + "." + dependency.Name,
                            ContainingAssembly = dependency.ContainingAssembly.Name
                        };

                        // collect mscorlib types as 'external'
                        if (dependency.ContainingAssembly.Name == "mscorlib")
                        {
                            var externalDep = externalTypes.FirstOrDefault(t => t.Name == dep.Name);

                            // check if not already registered
                            if (externalDep != null)
                            {
                                dep = externalDep;
                            }
                            else
                            {
                                externalTypes.Add(dep);
                            }
                        }

                        if (!graph.HasVertex(dep))
                        {
                            graph.AddVertex(dep);
                            graph.AddEdge(dependantType, dep);
                        }
                    }                   
                }
            }

            var vertices = graph.Vertices;
            var edges = graph.Edges;

            var readable = graph.ToReadable();
        }
    }
}
