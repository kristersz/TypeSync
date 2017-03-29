using Microsoft.CodeAnalysis;

namespace TypeSync.Core.Models.CSharp
{
    public class CSharpPropertyModel
    {
        public CSharpPropertyModel()
        {
            Type = new CSharpTypeModel();
        }

        public string Name { get; set; }

        public CSharpTypeModel Type { get; set; }
    }
}
