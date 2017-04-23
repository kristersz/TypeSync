using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace TypeSync.Core.Features
{
    public interface IAnalysisContext
    {
        MSBuildWorkspace Workspace { get; }

        Solution Solution { get; }

        Project Project { get; }

        Compilation Compilation { get; }

        void Init(string path);
    }
}
