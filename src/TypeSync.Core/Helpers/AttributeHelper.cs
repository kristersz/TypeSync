using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using TypeSync.Models.Common;

namespace TypeSync.Core.Helpers
{
    public static class AttributeHelper
    {
        public static string GetRoutePrefix(ImmutableArray<AttributeData> attributes)
        {
            if (!attributes.IsDefaultOrEmpty)
            {
                foreach (var attribute in attributes)
                {
                    if (attribute.AttributeClass.Name == "RoutePrefixAttribute")
                    {
                        return GetFirstAttributeArgument(attribute);
                    }
                }
            }

            return string.Empty;
        }

        public static string GetRoute(ImmutableArray<AttributeData> attributes)
        {
            if (!attributes.IsDefaultOrEmpty)
            {
                foreach (var attribute in attributes)
                {
                    if (attribute.AttributeClass.Name == "RouteAttribute")
                    {
                        return GetFirstAttributeArgument(attribute);
                    }
                }
            }

            return string.Empty;
        }

        public static HttpMethod? DetermineHttpMehod(ImmutableArray<AttributeData> attributes)
        {
            if (!attributes.IsDefaultOrEmpty)
            {
                foreach (var attribute in attributes)
                {
                    if (attribute.AttributeClass.Name == "HttpGetAttribute")
                    {
                        return HttpMethod.Get;
                    }
                    else if (attribute.AttributeClass.Name == "HttpPostAttribute")
                    {
                        return HttpMethod.Post;
                    }
                    else if (attribute.AttributeClass.Name == "HttpPutAttribute")
                    {
                        return HttpMethod.Put;
                    }
                    else if (attribute.AttributeClass.Name == "HttpPatchAttribute")
                    {
                        return HttpMethod.Patch;
                    }
                    else if (attribute.AttributeClass.Name == "HttpDeleteAttribute")
                    {
                        return HttpMethod.Delete;
                    }
                }
            }

            return null;
        }


        public static List<AttributeData> GetValidationAttributes(IPropertySymbol propertySymbol)
        {
            var result = new List<AttributeData>();

            var attributes = propertySymbol.GetAttributes();

            if (!attributes.IsDefaultOrEmpty)
            {
                foreach (var attribute in attributes)
                {
                    if (attribute.AttributeClass.BaseType.Name == "ValidationAttribute")
                    {
                        result.Add(attribute);
                    }
                }
            }

            return result;
        }


        public static string GetFirstAttributeArgument(AttributeData attribute)
        {
            var arguments = attribute.ConstructorArguments;

            if (!arguments.IsDefaultOrEmpty)
            {
                return arguments.First().Value as string;
            }

            return string.Empty;
        }
    }
}
