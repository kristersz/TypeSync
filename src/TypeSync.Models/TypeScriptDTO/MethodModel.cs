using TypeSync.Models.Common;

namespace TypeSync.Models.TypeScriptDTO
{
    public class MethodModel
    {
        public string Name { get; set; }
        public string ReturnType { get; set; }
        public bool IsHttpService { get; set; }
        public HttpMethod HttpMethod { get; set; }
        public ParameterModel[] Parameters { get; set; }
    }
}
