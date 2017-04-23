using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using TypeSync.Common.Constants;

namespace TypeSync.Core.Features.WebApiAnalysis
{
    public class WebApiAnalysisContext : BaseAnalysisContext
    {
        public WebApiAnalysisContext() : base() { }

        public override void Init(string path)
        {
            var extension = Path.GetExtension(path);

            if (extension == DotNetFileExtension.Solution)
            {
                _solution = _workspace.OpenSolutionAsync(path).Result;
                _project = _solution.Projects.FirstOrDefault(p => p.Name.Contains("WebAPI"));

                if (_project != null)
                {
                    _compilation = _project.GetCompilationAsync().Result;
                }
            }
            else if (extension == DotNetFileExtension.Project)
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
        }
    }
}
