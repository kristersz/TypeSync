using System.Collections.Generic;
using System.Linq;
using TypeSync.Models.CSharp;
using TypeSync.Models.TypeScript;

namespace TypeSync.Models.Converters
{
    public class ModelConverter
    {
        private readonly TypeConverter _typeConverter;

        public ModelConverter()
        {
            _typeConverter = new TypeConverter();
        }

        public TypeScriptClassModel ConvertClass(CSharpClassModel classModel)
        {
            return new TypeScriptClassModel()
            {
                Name = classModel.Name,
                BaseClass = classModel.BaseClass,
                IsGeneric = classModel.IsGeneric,
                TypeParameters = classModel.TypeParameters.Select(tp => new TypeScriptTypeParameterModel()
                {
                    Name = tp.Name,
                }).ToList(),
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
                    Type = _typeConverter.ConvertType(p.Type)
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
    }
}
