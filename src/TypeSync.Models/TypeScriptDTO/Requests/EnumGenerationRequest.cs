namespace TypeSync.Models.TypeScriptDTO
{
    public class EnumGenerationRequest
    {
        public string OutputPath { get; set; }
        public EnumModel DataModel { get; set; }
    }
}
