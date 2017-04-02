namespace TypeSync.Models.TypeScript
{
    public class TypeScriptPropertyModel
    {
        public TypeScriptPropertyModel()
        {
            Type = new TypeScriptTypeModel();
        }

        public string Name { get; set; }

        public bool IsOptional { get; set; }

        public TypeScriptTypeModel Type { get; set; }
    }
}
