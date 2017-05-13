namespace TypeSync.Models.TypeScriptDTO
{
    public class MethodModel
    {
        public string Name { get; set; }
        public string ReturnType { get; set; }
        public ParameterModel[] Parameters { get; set; }
    }
}
