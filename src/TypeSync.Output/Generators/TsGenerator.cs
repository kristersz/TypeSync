using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TypeSync.Common.Constants;
using TypeSync.Common.Utilities;
using TypeSync.Models.Angular;
using TypeSync.Models.Common;
using TypeSync.Models.TypeScript;
using TypeSync.Models.TypeScriptDTO;

namespace TypeSync.Output.Generators
{
    public class TsGenerator
    {
        private HttpClient _client;

        public TsGenerator()
        {
            _client = new HttpClient();

            _client.BaseAddress = new Uri("http://localhost:8080");
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private string CallGenerator(string path, HttpContent content)
        {
            var response = _client.PostAsync(path, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsStringAsync().Result;
            }

            return string.Empty;
        }

        private StringContent CreateStringContent(object value)
        {
            return new StringContent(
                JsonConvert.SerializeObject(
                    value, 
                    new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    }).ToString(), 
                Encoding.UTF8, 
                "application/json"
            );
        }

        public void GenerateDataModelAST(TypeScriptClassModel classModel, string outputPath)
        {
            var request = new ClassGenerationRequest();

            string fileName = $"{NameCaseConverter.ToKebabCase(classModel.Name)}.model.{TypeScriptFileExtension.File}";

            var typeGenerator = new TypeGenerator();

            request.OutputPath = Path.Combine(outputPath, "models", fileName);

            request.DataModel = new ClassModel()
            {
                Name = classModel.Name,
                BaseClass = classModel.BaseClass,
                Decorators = new string[] { },
                TypeParameters = classModel.TypeParameters.Select(i => i.Name).ToArray(),
                Imports = classModel.Imports.Select(i => new ImportModel()
                {
                    Names = new string[] { i.Name },
                    Path = i.DependencyKind == DependencyKind.Model 
                        ? $"./{NameCaseConverter.ToKebabCase(i.Name)}.model" 
                        : $"../enums/{NameCaseConverter.ToKebabCase(i.Name)}.enum"
                }).ToArray(),
                Properties = classModel.Properties.Select(p => new PropertyModel()
                {
                    Name = NameCaseConverter.ToCamelCase(p.Name),
                    Type = typeGenerator.GetEmittedType(p.Type),
                    IsPrivate = false,
                    InitialValue = null,
                }).ToArray(),
            };

            var result = CallGenerator("/generate/class", CreateStringContent(request));
        }

        public void GenerateEnumAST(TypeScriptEnumModel enumModel, string outputPath)
        {
            var request = new EnumGenerationRequest();

            string fileName = $"{NameCaseConverter.ToKebabCase(enumModel.Name)}.enum.{TypeScriptFileExtension.File}";

            request.OutputPath = Path.Combine(outputPath, "enums", fileName);

            request.DataModel = new EnumModel()
            {
                Name = enumModel.Name,
                Members = enumModel.Members.Select(m => new EnumMemberModel()
                {
                    Name = m.Name,
                    Value = m.Value?.ToString()
                }).ToArray()
            };

            var result = CallGenerator("/generate/enum", CreateStringContent(request));
        }

        public void GenerateServiceAST(TypeScriptServiceModel serviceModel, string outputPath)
        {
            var request = new ClassGenerationRequest();

            string fileName = $"{NameCaseConverter.ToKebabCase(serviceModel.Name)}.service.{TypeScriptFileExtension.File}";

            var typeGenerator = new TypeGenerator();

            request.OutputPath = Path.Combine(outputPath, "services", fileName);

            var imports = new List<ImportModel>() {
                new ImportModel() { Names = new string[] { "Injectable" }, Path = "@angular/core" },
                new ImportModel() { Names = new string[] { "Http", "Headers", "Response" }, Path = "@angular/http" },
                new ImportModel() { Names = new string[] { }, Path = "rxjs/add/operator/toPromise" }
            };

            var specificImports = serviceModel.Imports.Select(i => new ImportModel()
            {
                Names = new string[] { i.Name },
                Path = i.DependencyKind == DependencyKind.Model 
                    ? $"../models/{NameCaseConverter.ToKebabCase(i.Name)}.model" 
                    : $"../enums/{NameCaseConverter.ToKebabCase(i.Name)}.enum"
            }).ToList();

            imports.AddRange(specificImports);

            request.DataModel = new ClassModel()
            {
                Name = serviceModel.Name + "Service",
                BaseClass = null,
                Decorators = new string[] { "Injectable()" },
                TypeParameters = new string[] { },
                Imports = imports.ToArray(),
                Properties = new PropertyModel[]
                {
                    new PropertyModel() { Name = "baseUrl", Type = null, IsPrivate = true, InitialValue = serviceModel.RoutePrefix }
                },
                ConstructorDef = new ConstructorModel()
                {
                    Parameters = new ParameterModel[]
                    {
                        new ParameterModel() { Name = "http", Type = "Http", IsPrivate = true }
                    }
                },
                Methods = serviceModel.Methods.Select(m => new HttpMethodModel()
                {
                    Name = NameCaseConverter.ToCamelCase(m.Name),
                    ReturnType = typeGenerator.GetEmittedType(m.ReturnType),
                    HttpMethod = m.HttpMethod,
                    Route = m.Route,
                    Parameters = m.Parameters.Select(p => new ParameterModel()
                    {
                        Name = p.Item2,
                        Type = typeGenerator.GetEmittedType(p.Item1),
                        IsPrivate = false
                    }).ToArray()
                }).ToArray()
            };

            var result = CallGenerator("/generate/class", CreateStringContent(request));
        }

        public void GenerateValidatorAST(AngularFormValidatorModel validatorModel, string outputPath)
        {
            var request = new ClassGenerationRequest();

            string fileName = $"{NameCaseConverter.ToKebabCase(validatorModel.Name)}.validator.{TypeScriptFileExtension.File}";

            var typeGenerator = new TypeGenerator();

            request.OutputPath = Path.Combine(outputPath, "validators", fileName);

            var imports = new List<ImportModel>() {
                new ImportModel() { Names = new string[] { "FormGroup, FormBuilder, Validators" }, Path = "@angular/forms" }
            };

            var specificImports = validatorModel.Imports.Select(i => new ImportModel()
            {
                Names = new string[] { i.Name },
                Path = i.DependencyKind == DependencyKind.Model
                    ? $"../models/{NameCaseConverter.ToKebabCase(i.Name)}.model"
                    : $"../enums/{NameCaseConverter.ToKebabCase(i.Name)}.enum"
            }).ToList();

            imports.AddRange(specificImports);

            request.DataModel = new ClassModel()
            {
                Name = validatorModel.Name + "Validator",
                BaseClass = null,
                Decorators = new string[] { },
                TypeParameters = new string[] { },
                Imports = imports.ToArray(),
                Properties = new PropertyModel[]
                {
                    new PropertyModel() { Name = "validationMessages", Type = null, IsPrivate = false, InitialValue = "" }
                },
                ConstructorDef = new ConstructorModel()
                {
                    Parameters = new ParameterModel[]
                    {
                        new ParameterModel() { Name = "fb", Type = "FormBuilder", IsPrivate = false }
                    }
                },
                Methods = new MethodModel[]
                {
                    new ValidatorBuilderMethodModel()
                    {

                    }
                }
            };

            var result = CallGenerator("/generate/class", CreateStringContent(request));
        }
    }
}
