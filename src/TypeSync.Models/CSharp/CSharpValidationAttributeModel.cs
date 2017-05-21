namespace TypeSync.Models.CSharp
{
    public class CSharpValidationAttributeModel
    {
        public CSharpValidationAttribute ValidationAttribute { get; set; }

        public string PropertyName { get; set; }

        public string ErrorMessage { get; set; }

        public object Argument { get; set; }
    }
}
