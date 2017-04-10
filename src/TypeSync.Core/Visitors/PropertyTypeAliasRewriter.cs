using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TypeSync.Core.Visitors
{
    public class PropertyTypeAliasRewriter : CSharpSyntaxRewriter
    {
        private IDictionary<string, string> _aliasDictionary = new Dictionary<string, string>()
        {
            { "Boolean", "bool" },
            { "Byte", "byte" },
            { "SByte", "sbyte" },
            { "Char", "char" },
            { "Decimal", "decimal" },
            { "Double", "double" },
            { "Single", "float" },
            { "Int32", "int" },
            { "UInt32", "uint" },
            { "Int64", "long" },
            { "UInt64 ", "ulong" },
            { "Object", "object" },
            { "Int16 ", "short" },
            { "UInt16", "ushort" },
            { "String", "string" }
        };

        public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            if (!node.Type.IsKind(SyntaxKind.PredefinedType))
            {               
                string identifierName = node.Type.ToString();

                // not a predefined type, check if the type is a qualified name like System.String
                if (node.Type.IsKind(SyntaxKind.QualifiedName))
                {
                    // get all the identifier names seperated by dots
                    var identifierNames = node.Type.ChildNodes().OfType<IdentifierNameSyntax>().ToList();

                    if (identifierNames.Any())
                    {
                        // take the last one for simplicity
                        identifierName = identifierNames.Last().ToString();
                    }
                }

                // look for an alias from a known dictionary of type aliases
                string alias;

                if (_aliasDictionary.TryGetValue(identifierName, out alias))
                {
                    // alias found, rewrite the syntax with it
                    return node.WithType(SyntaxFactory.ParseTypeName(alias).WithTrailingTrivia(node.Type.GetTrailingTrivia()));
                }
            }

            // already a predefined type, return as is
            return node;
        }
    }
}
