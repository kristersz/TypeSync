using TypeSync.Models.Common;

namespace TypeSync.Models.TypeScript
{
    public class TypeScriptImportModel
    {
        public string Name { get; set; }

        public string FilePath { get; set; }

        public DependencyKind DependencyKind { get; set; }
    }
}
