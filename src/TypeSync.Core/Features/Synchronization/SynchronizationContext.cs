using System;
using System.IO;
using System.Linq;
using TypeSync.Common.Constants;

namespace TypeSync.Core.Features.Synchronization
{
    public class SynchronizationContext : AnalysisContextBase
    {
        public SynchronizationContext() : base() { }

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
                throw new NotSupportedException("The extension is not supported for this scenario");
            }
        }
    }
}
