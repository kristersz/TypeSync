using System;

namespace TypeSync.Core.Helpers
{
    public static class ControllerHelper
    {
        public static string ExtractControllerName(string controllerFullName)
        {
            if (!string.IsNullOrWhiteSpace(controllerFullName))
            {
                int charLocation = controllerFullName.IndexOf("Controller", StringComparison.Ordinal);

                if (charLocation > 0)
                {
                    return controllerFullName.Substring(0, charLocation);
                }
            }

            return string.Empty;
        }
    }
}
