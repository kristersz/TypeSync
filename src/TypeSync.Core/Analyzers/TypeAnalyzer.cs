using System.Collections.Generic;
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
                    typeModel.IsArray = true;
                    typeModel.ElementType = CreateTypeModel(arrayTypeSymbol.ElementType);
                }
            }
            else if (typeSymbol is INamedTypeSymbol)
            {
                var namedTypeSymbol = typeSymbol as INamedTypeSymbol;

                if (namedTypeSymbol.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T)
                {
                    // nullable types
                    typeModel.IsNullable = true;

                    if (!namedTypeSymbol.TypeArguments.IsDefaultOrEmpty)
                    {
                        var typeArgument = namedTypeSymbol.TypeArguments[0];

                        typeModel.TypeArguments = new List<CSharpTypeModel>() { CreateTypeModel(typeArgument) };
                    }
                }
               
                if (namedTypeSymbol != null 
                    && (namedTypeSymbol.ConstructedFrom.Equals(IEnumerableTypeSymbol) || namedTypeSymbol.AllInterfaces.Any(i => i.ConstructedFrom.Equals(IEnumerableTypeSymbol))))
                {

                    // enumerable types such as List<T>, IList<T> or IEnumerable<T>
                    typeModel.IsCollection = true;

                    if (!namedTypeSymbol.TypeArguments.IsDefaultOrEmpty)
                    {
                        var typeArgument = namedTypeSymbol.TypeArguments[0];

                        typeModel.TypeArguments = new List<CSharpTypeModel>() { CreateTypeModel(typeArgument) };
                    }
                }
            }

            return typeModel;
        }

        private CSharpTypeModel CreateTypeModel(ITypeSymbol typeSymbol)
        {
            return new CSharpTypeModel()
            {
                Name = typeSymbol.Name,
                TypeKind = TypeMapper.MapTypeKind(typeSymbol.TypeKind),
                SpecialType = TypeMapper.MapSpecialType(typeSymbol.SpecialType)
            };
        }
    }
}
