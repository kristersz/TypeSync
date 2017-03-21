using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TypeSync.SyntaxWalkers
{
    public class ClassWalker : CSharpSyntaxWalker
    {
        public readonly List<ClassDeclarationSyntax> Classes = new List<ClassDeclarationSyntax>();

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            Classes.Add(node);

            base.VisitClassDeclaration(node);
        }
    }
}
