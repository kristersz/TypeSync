using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace TypeSync.Core.Visitors
{
    public class NamedTypeSymbolFinder
    {
        public List<INamedTypeSymbol> GetAllSymbols(Compilation compilation)
        {
            var visitor = new NamedTypeSymbolVisitor();

            visitor.Visit(compilation.GlobalNamespace);

            return visitor.NamedTypeSymbols;
        }

        private class NamedTypeSymbolVisitor : SymbolVisitor
        {
            public List<INamedTypeSymbol> NamedTypeSymbols { get; } = new List<INamedTypeSymbol>();

            public override void VisitNamespace(INamespaceSymbol symbol)
            {
                Parallel.ForEach(symbol.GetMembers(), s => s.Accept(this));
            }

            public override void VisitNamedType(INamedTypeSymbol symbol)
            {
                NamedTypeSymbols.Add(symbol);

                foreach (var childSymbol in symbol.GetTypeMembers())
                {
                    base.Visit(childSymbol);
                }
            }
        }
    }
}
