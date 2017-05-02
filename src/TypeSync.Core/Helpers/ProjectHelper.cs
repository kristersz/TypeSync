using Microsoft.CodeAnalysis;

namespace TypeSync.Core.Helpers
{
    public static class ProjectHelper
    {
        public static bool IsDotNetCoreProject(Compilation compilation)
        {
            var coreAssembly = compilation.GetTypeByMetadataName("System.Object").ContainingAssembly;

            if (coreAssembly.Name == "mscorlib")
            {
                // full .net framework defines System.Object in mscorlib
                return false;
            }
            else
            {
                // .net core defines System.Object in System.Runtime
                return true;
            }
        }
    }
}
