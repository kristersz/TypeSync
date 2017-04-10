using System.Collections.Generic;
using System.Linq;
using TypeSync.Models.CSharp;
using TypeSync.Models.TypeScript;

namespace TypeSync.Output.Converters
{
    public class ModelConverter
    {
        public List<TypeScriptClassModel> ConvertClasses(List<CSharpClassModel> classModels)
        {
            return classModels.Select(c => new TypeScriptClassModel()
            {
                Name = c.Name,
                BaseClass = c.BaseClass,
                Imports = c.Dependencies.Select(d => new TypeScriptImportModel()
                {
                    Name = d.Name,
                    FilePath = ""
                }).ToList(),
                Properties = c.Properties.Select(p => new TypeScriptPropertyModel()
                {
                    Name = p.Name,
                    IsOptional = p.Type.IsNullable,
                    Type = ConvertType(p.Type)
                }).ToList()
            }).ToList();
        }

        public List<TypeScriptEnumModel> ConvertEnums(List<CSharpEnumModel> enumModels)
        {
            return enumModels.Select(c => new TypeScriptEnumModel()
            {
                Name = c.Name,
                Members = c.Members.Select(p => new TypeScriptEnumMemberModel()
                {
                    Name = p.Name,
                    Value = p.Value
                }).ToList()
            }).ToList();
        }

        public TypeScriptTypeModel ConvertType(CSharpTypeModel csTypeModel)
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
