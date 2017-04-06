using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.MSBuild;
using TypeSync.Common.Constants;

namespace TypeSync.Core.Features.ModelAnalysis
{
    public class ModelAnalysisContext
    {
        private readonly MSBuildWorkspace _workspace;
        private Solution _solution;
        private Project _project;
        private Compilation _compilation;

        public MSBuildWorkspace Workspace => _workspace;

        public Solution Solution => _solution;

        public Project Project => _project;

        public Compilation Compilation => _compilation;

        public ModelAnalysisContext()
        {
            _workspace = MSBuildWorkspace.Create();
        }

        public void Init(string path)
        {
            var extension = Path.GetExtension(path);

            if (extension == FileExtension.Solution)
            {
                _solution = _workspace.OpenSolutionAsync(path).Result;
                _project = _solution.Projects.FirstOrDefault(p => p.Name.Contains("ViewModels"));

                if (_project != null)
                {
                    _compilation = _project.GetCompilationAsync().Result;
                }
            }
            else if (extension == FileExtension.Project)
            {
                _project = _workspace.OpenProjectAsync(path).Result;
                _solution = _project.Solution;
                _compilation = _project.GetCompilationAsync().Result;
            }
            else
            {
                // parse the syntax tree and build a compilation
                var text = File.ReadAllText(path);
                var tree = CSharpSyntaxTree.ParseText(text).WithFilePath(path);
                var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);

                _compilation = CSharpCompilation.Create(
                    "MyCompilation", 
                    syntaxTrees: new[] { tree }, 
                    references: new[] { mscorlib }
                );
            }

            _compilation = _project.GetCompilationAsync().Result;
        }
    }
}
