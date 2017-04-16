using TypeSync.Models.Common;

namespace TypeSync.Models.CSharp
{
    public class CSharpDependencyModel
    {
        public string Name { get; set; }

        public string Namespace { get; set; }

        public DependencyKind DependencyKind { get; set; }
    }
}
