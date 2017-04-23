using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace TypeSync.Core.Features
{
    public abstract class AnalysisContextBase : IAnalysisContext
    {
        protected readonly MSBuildWorkspace _workspace;
        protected Solution _solution;
        protected Project _project;
        protected Compilation _compilation;

        public MSBuildWorkspace Workspace => _workspace;

        public Solution Solution => _solution;

        public Project Project => _project;

        public Compilation Compilation => _compilation;

        public AnalysisContextBase()
        {
            _workspace = MSBuildWorkspace.Create();
        }

        public virtual void Init(string path)
        {
        }
    }
}
