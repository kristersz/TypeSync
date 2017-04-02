using System.Linq;
using TypeSync.Models.CSharp;
using TypeSync.Models.Enums;
using TypeSync.Models.TypeScript;

namespace TypeSync.Output.Converters
{
    public class ModelConverter
    {
        public TypeScriptClassModel ConvertClassModel(CSharpClassModel classModel)
        {
            return new TypeScriptClassModel()
            {
                Name = classModel.Name,
                Properties = classModel.Properties.Select(p => new TypeScriptPropertyModel()
                {
                    Name = p.Name,
                    IsOptional = p.Type.IsNullable,
                    Type = ConvertTypeModel(p.Type)
                }).ToList()
            };
        }

        private TypeScriptTypeModel ConvertTypeModel(CSharpTypeModel csTypeModel)
        {
            var tsTypeModel = CreateTypeModel(csTypeModel);

            if (csTypeModel.IsArray)
            {
                tsTypeModel.ElementType = CreateTypeModel(csTypeModel.ElementType);
            }
            else if (csTypeModel.IsCollection)
            {
                tsTypeModel.ElementType = CreateTypeModel(csTypeModel.TypeArguments.First());
            }
            else if (csTypeModel.IsNullable)
            {
                tsTypeModel = CreateTypeModel(csTypeModel.TypeArguments.First());
            }

            return tsTypeModel;
        }

        private TypeScriptTypeModel CreateTypeModel(CSharpTypeModel csTypeModel)
        {
            return new TypeScriptTypeModel()
            {
                Name = csTypeModel.Name,
                IsNamedType = csTypeModel.SpecialType == CSharpSpecialType.None,
                PredefinedType = TypeMapper.MapCSharpTypeToTypeScript(csTypeModel.SpecialType)
            };
        }
    }
}
