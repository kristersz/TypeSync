using System;
using System.Collections.Generic;
using System.Linq;
using DataStructures.Graphs;
using Microsoft.CodeAnalysis;
using TypeSync.Core.Helpers;

namespace TypeSync.Core.Factories
{
    public class DependencyNode : IComparable<DependencyNode>
    {
        public INamedTypeSymbol NamedTypeSymbol { get; set; }

        public override string ToString()
        {
            return $"{NamedTypeSymbol.Name}";
        }


        public int CompareTo(DependencyNode other)
        {
            return this.ToString().CompareTo(other.ToString());
        }
    }

    public class DependencyGraphFactory
    {
        public DirectedSparseGraph<DependencyNode> Graph { get; private set; }

        public DependencyGraphFactory()
        {
            Graph = new DirectedSparseGraph<DependencyNode>();
        }

        public DirectedSparseGraph<DependencyNode> BuildForNamedTypeSymbol(INamedTypeSymbol namedType)
        {
            GetDependenciesRecursive(namedType, null);

            return Graph;
        }

        private void GetDependenciesRecursive(INamedTypeSymbol classSymbol, DependencyNode parent)
        {
            var node = new DependencyNode() { NamedTypeSymbol = classSymbol };
            var existingNode = Graph.Vertices.FirstOrDefault(v => v.NamedTypeSymbol.Name == classSymbol.Name);

            if (existingNode != null)
            {
                node = existingNode;
            }
            else
            {
                Graph.AddVertex(node);
            }

            if (parent != null)
            {
                Graph.AddEdge(parent, node);
            }

            var dependencies = GetDependencies(classSymbol);

            foreach (var dep in dependencies)
            {
                GetDependenciesRecursive(dep, node);
            }
        }

        private List<INamedTypeSymbol> GetDependencies(INamedTypeSymbol classSymbol)
        {
            var dependencies = new List<INamedTypeSymbol>();

            if (classSymbol.BaseType != null && TypeHelper.IsSupportedType(classSymbol.BaseType))
            {
                dependencies.Add(classSymbol.BaseType);
            }

            var members = classSymbol.GetMembers().ToList();

            var properties = members.Where(m => m.Kind == SymbolKind.Property).ToList();

            foreach (var property in properties)
            {
                var propertySymbol = property as IPropertySymbol;

                if (TypeHelper.IsSupportedType(propertySymbol.Type))
                {
                    dependencies.Add(propertySymbol.Type as INamedTypeSymbol);
                }
            }

            return dependencies;
        }
    }
}
