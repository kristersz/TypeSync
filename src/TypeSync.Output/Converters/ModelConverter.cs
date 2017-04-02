using System.Linq;
using TypeSync.Models.CSharp;
using TypeSync.Models.Enums;
using TypeSync.Models.TypeScript;

namespace TypeSync.Output.Converters
{
    public class ModelConverter
    {
        public TypeScriptClassModel ConvertModel(CSharpClassModel classModel)
        {
            return new TypeScriptClassModel()
            {
                Name = classModel.Name,
                Properties = classModel.Properties.Select(p => new TypeScriptPropertyModel()
                {
                    Name = p.Name,
                    Type = ConvertType(p.Type)
                }).ToList()
            };
        }

        private TypeScriptTypeModel ConvertType(CSharpTypeModel csTypeModel)
        {
            var tsTypeModel = new TypeScriptTypeModel();

            tsTypeModel.Name = csTypeModel.Name;
            tsTypeModel.IsNamedType = csTypeModel.SpecialType == CSharpSpecialType.None;
            tsTypeModel.PredefinedType = TypeConverter.ConvertCSharpTypeToTypeScript(csTypeModel.SpecialType);

            if (csTypeModel.IsCollection)
            {
                tsTypeModel.ElementType = new TypeScriptTypeModel();

                tsTypeModel.ElementType.Name = csTypeModel.ElementType.Name;
                tsTypeModel.ElementType.IsNamedType = csTypeModel.ElementType.SpecialType == CSharpSpecialType.None;
                tsTypeModel.ElementType.PredefinedType = TypeConverter.ConvertCSharpTypeToTypeScript(csTypeModel.ElementType.SpecialType);
            }

            return tsTypeModel;
        }
    }
}
