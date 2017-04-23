using System.Collections.Generic;
using System.Linq;
using TypeSync.Models.CSharp;
using TypeSync.Models.TypeScript;

namespace TypeSync.Models.Converters
{
    public class ModelConverter
    {
        public TypeScriptClassModel ConvertClass(CSharpClassModel classModel)
        {
            return new TypeScriptClassModel()
            {
                Name = classModel.Name,
                BaseClass = classModel.BaseClass,
                IsGeneric = classModel.IsGeneric,
                TypeParameter = classModel.TypeParameter == null ? null : new TypeScriptTypeParameterModel() { Name = classModel.TypeParameter.Name },
                Imports = classModel.Dependencies.Select(d => new TypeScriptImportModel()
                {
                    Name = d.Name,
                    FilePath = "",
                    DependencyKind = d.DependencyKind
                }).ToList(),
                Properties = classModel.Properties.Select(p => new TypeScriptPropertyModel()
                {
                    Name = p.Name,
                    IsOptional = p.Type.IsNullable,
                    Type = ConvertType(p.Type)
                }).ToList()
            };
        }

        public List<TypeScriptClassModel> ConvertClasses(List<CSharpClassModel> classModels)
        {
            return classModels
                .Select(c => ConvertClass(c))
                .ToList();
        }


        public TypeScriptEnumModel ConvertEnum(CSharpEnumModel enumModel)
        {
            return new TypeScriptEnumModel()
            {
                Name = enumModel.Name,
                Members = enumModel.Members.Select(p => new TypeScriptEnumMemberModel()
                {
                    Name = p.Name,
                    Value = p.Value
                }).ToList()
            };
        }

        public List<TypeScriptEnumModel> ConvertEnums(List<CSharpEnumModel> enumModels)
        {
            return enumModels.Select(c => ConvertEnum(c)).ToList();
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
                PredefinedType = TypeConverter.MapCSharpTypeToTypeScript(csTypeModel.SpecialType)
            };
        }
    }
}
