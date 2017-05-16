using Microsoft.CodeAnalysis;

namespace TypeSync.Core.Helpers
{
    public static class TypeHelper
    {
        public static bool IsSupportedType(ITypeSymbol typeSymbol)
        {
            if (typeSymbol.ContainingAssembly.Name == "mscorlib")
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
    }
}
