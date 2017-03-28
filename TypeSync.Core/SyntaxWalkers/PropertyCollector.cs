using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TypeSync.Core.SyntaxWalkers
{
    public class PropertyCollector : CSharpSyntaxWalker
    {
        public readonly List<PropertyDeclarationSyntax> Properties = new List<PropertyDeclarationSyntax>();

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            Properties.Add(node);
        }
    }
}
