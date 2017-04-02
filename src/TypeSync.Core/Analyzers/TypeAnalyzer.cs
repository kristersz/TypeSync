using System.Linq;
using log4net;
using Microsoft.CodeAnalysis;
using TypeSync.Core.Mappers;
using TypeSync.Models.CSharp;
using TypeSync.Models.Enums;

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
            typeModel.TypeKind = TypeMapper.MapTypeKind(typeSymbol.TypeKind);


            if (typeSymbol.SpecialType != SpecialType.None)
            {
                // special types
                typeModel.SpecialType = TypeMapper.MapSpecialType(typeSymbol.SpecialType);
            }
            else if (typeModel.TypeKind == CSharpTypeKind.Array)
            {
                // arrays
                var arrayTypeSymbol = typeSymbol as IArrayTypeSymbol;

                if (arrayTypeSymbol != null)
                {
                    typeModel.IsCollection = true;
                    typeModel.ElementType = new CSharpTypeModel()
                    {
                        Name = arrayTypeSymbol.ElementType.Name,
                        TypeKind = TypeMapper.MapTypeKind(arrayTypeSymbol.ElementType.TypeKind),
                        SpecialType = TypeMapper.MapSpecialType(arrayTypeSymbol.ElementType.SpecialType)
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
                            TypeKind = TypeMapper.MapTypeKind(typeArgument.TypeKind),
                            SpecialType = TypeMapper.MapSpecialType(typeArgument.SpecialType)
                        };
                    }
                }
            }

            return typeModel;
        }
    }
}
