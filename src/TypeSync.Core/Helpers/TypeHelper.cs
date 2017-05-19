using Microsoft.CodeAnalysis;

namespace TypeSync.Core.Helpers
{
    public static class TypeHelper
    {
        public static bool IsSupportedType(ITypeSymbol typeSymbol)
        {
            var assemblyName = typeSymbol.ContainingAssembly.Name;

            if (assemblyName == "mscorlib" || assemblyName.StartsWith("System"))
            {
                if (typeSymbol.MetadataName == SupportedDotNetTypes.KeyValuePair)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsNullable(INamedTypeSymbol namedTypeSymbol)
        {
            return namedTypeSymbol.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T;
        }
    }
}
