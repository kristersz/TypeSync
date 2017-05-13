using TypeSync.Models.Common;

namespace TypeSync.Models.TypeScriptDTO
{
    public class HttpMethodModel : MethodModel
    {
        public HttpMethod HttpMethod { get; set; }

        public string Route { get; set; }
    }
}
