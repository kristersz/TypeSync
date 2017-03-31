using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Microsoft.CodeAnalysis;
using TypeSync.Core.Models.CSharp;

namespace TypeSync.Core.Analyzers
{
    public class TypeAnalyzer
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TypeAnalyzer));

        public TypeAnalyzer()
        {
        }

        public CSharpTypeModel AnalyzePropertyType(ITypeSymbol typeSymbol, INamedTypeSymbol IEnumerableTypeSymbol)
        {
            var typeModel = new CSharpTypeModel();

            typeModel.Name = typeSymbol.Name;
            typeModel.TypeKind = typeSymbol.TypeKind;


            if (typeSymbol.SpecialType != SpecialType.None)
            {
                // special types
                typeModel.SpecialType = typeSymbol.SpecialType;
            }
            else if (typeModel.TypeKind == TypeKind.Array)
            {
                // arrays
                var arrayTypeSymbol = typeSymbol as IArrayTypeSymbol;

                if (arrayTypeSymbol != null)
                {
                    typeModel.IsCollection = true;
                    typeModel.ElementType = new CSharpTypeModel()
                    {
                        Name = arrayTypeSymbol.ElementType.Name,
                        TypeKind = arrayTypeSymbol.ElementType.TypeKind,
                        SpecialType = arrayTypeSymbol.ElementType.SpecialType
                    };
                }
            }
            else if (typeSymbol is INamedTypeSymbol)
            {
                var namedTypeSymbol = typeSymbol as INamedTypeSymbol;

                // enumerable types such as List<T>, IList<T> or IEnumerable<T>
                if (namedTypeSymbol != null 
                    && (namedTypeSymbol.ConstructedFrom.Equals(IEnumerableTypeSymbol) || namedTypeSymbol.AllInterfaces.Any(i => i.ConstructedFrom.Equals(IEnumerableTypeSymbol))))
                {
                    typeModel.IsCollection = true;

                    if (!namedTypeSymbol.TypeArguments.IsDefaultOrEmpty)
                    {
                        var typeArgument = namedTypeSymbol.TypeArguments[0];

                        typeModel.ElementType = new CSharpTypeModel()
                        {
                            Name = typeArgument.Name,
                            TypeKind = typeArgument.TypeKind,
                            SpecialType = typeArgument.SpecialType
                        };
                    }
                }
            }

            return typeModel;
        }
    }
}
