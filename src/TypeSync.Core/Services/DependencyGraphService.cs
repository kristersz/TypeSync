using System;
using System.Linq;
using DataStructures.Graphs;
using Microsoft.CodeAnalysis;
using TypeSync.Core.Helpers;

namespace TypeSync.Core.Services
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

    public class DependencyGraphService
    {
        public DirectedSparseGraph<DependencyNode> Graph { get; private set; }

        public DependencyGraphService()
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
            var node = new DependencyNode()
            {
                NamedTypeSymbol = classSymbol
            };

            var existingNode = Graph.Vertices
                .FirstOrDefault(v => v.NamedTypeSymbol.Name == classSymbol.Name);

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

            var depService = new DependencyService();

            var dependencies = depService.GetTypeDependencies(classSymbol);

            foreach (var dep in dependencies)
            {
                GetDependenciesRecursive(dep, node);
            }
        }
    }
}
