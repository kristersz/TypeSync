using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TypeSync.Models;

namespace TypeSync.SyntaxWalkers
{
    public class PropertyWalker : CSharpSyntaxWalker
    {
        public readonly List<Property> Properties = new List<Property>();

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            Properties.Add(new Property()
            {
                Name = node.Identifier.Text,
                Type = node.Type.ToString()
            });
        }
    }
}
