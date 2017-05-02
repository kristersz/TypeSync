using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using TypeSync.Core.Features;

namespace TypeSync.Tests.Infrastructure.TestDoubles
{
    public class TestAnalysisContext : IAnalysisContext
    {
        protected readonly MSBuildWorkspace _workspace;
        protected Solution _solution;
        protected Project _project;
        protected Compilation _compilation;

        public MSBuildWorkspace Workspace => _workspace;

        public Solution Solution => _solution;

        public Project Project => _project;

        public Compilation Compilation => _compilation;

        public TestAnalysisContext(Compilation compilation)
        {
            _compilation = compilation;
        }

        public virtual void Init(string path)
        {
        }
    }
}
