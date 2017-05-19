using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using TypeSync.Core.Helpers;

namespace TypeSync.Core.Services
{
    public class DependencyService
    {
        private List<INamedTypeSymbol> _dependencies = new List<INamedTypeSymbol>();

        public List<INamedTypeSymbol> GetTypeDependencies(INamedTypeSymbol classSymbol)
        {
            if (classSymbol.BaseType != null)
            {
                ProccessType(classSymbol.BaseType);
            }

            var members = classSymbol.GetMembers().ToList();

            var properties = members.Where(m => m.Kind == SymbolKind.Property).ToList();

            foreach (var property in properties)
            {
                var propertySymbol = property as IPropertySymbol;

                ProccessType(propertySymbol.Type);
            }

            return _dependencies;
        }

        private void ProccessType(ITypeSymbol typeSymbol)
        {
            if (typeSymbol is IArrayTypeSymbol)
            {
                var arrayTypeSymbol = typeSymbol as IArrayTypeSymbol;

                ProccessType(arrayTypeSymbol.ElementType);
            }
            else if (typeSymbol is ITypeParameterSymbol)
            {
                var typeParameterSymbol = typeSymbol as ITypeParameterSymbol;

                return;
            }
            else if (typeSymbol is INamedTypeSymbol)
            {
                var namedTypeSymbol = typeSymbol as INamedTypeSymbol;

                if (!namedTypeSymbol.TypeArguments.IsDefaultOrEmpty)
                {
                    foreach (var typeArgument in namedTypeSymbol.TypeArguments)
                    {
                        ProccessType(typeArgument);
                    }
                }
                else
                {
                    if (TypeHelper.IsSupportedType(namedTypeSymbol))
                    {
                        _dependencies.Add(namedTypeSymbol);
                    }
                }
            }           
        }
    }
}
