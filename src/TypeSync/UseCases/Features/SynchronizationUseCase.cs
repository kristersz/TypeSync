using System;
using log4net;
using TypeSync.Core.Features.Synchronization;
using TypeSync.Models;
using TypeSync.Models.Converters;
using TypeSync.Output.Emitters;
using TypeSync.Output.Generators;

namespace TypeSync.UseCases
{
    public class SynchronizationUseCase : IUseCase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ModelGenerationUseCase));

        private Configuration _configuration;

        public UseCase Id => UseCase.Synchronization;

        public string Description => "Synchronize Angular client side with ASP.NET server side";

        public SynchronizationUseCase(Configuration configuration)
        {
            _configuration = configuration;
        }

        public Result Handle()
        {
            var syncer = new Synchronizer();

            var modelConverter = new ModelConverter();
            var webApiConverter = new WebApiConverter();

            var modelGenerator = new ModelGenerator();
            var webClientGenerator = new WebClientGenerator();
            var tsGenerator = new TsGenerator();

            var emitter = new TypeScriptEmitter();

            Generator generatorEnum;
            if (!Enum.TryParse(_configuration.Generator, true, out generatorEnum))
            {
                return Result.CreateError("Unknown generator was specified");
            }

            var analysisResult = syncer.Synchronize(_configuration.InputPath);

            if (!analysisResult.Success)
            {
                return Result.CreateError($"Source analysis error: {analysisResult.ErrorMessage}");
            }

            log.Debug("Source analyzed");

            var tsClassModels = modelConverter.ConvertClasses(analysisResult.Value.DataModels.Classes);
            var tsEnumModels = modelConverter.ConvertEnums(analysisResult.Value.DataModels.Enums);
            var serviceModels = webApiConverter.ConvertControllers(analysisResult.Value.Controllers);

            log.Debug("Models converted");

            foreach (var tsModel in tsClassModels)
            {
                if (generatorEnum == Generator.Template)
                {
                    var contents = modelGenerator.GenerateClass(tsModel);
                    emitter.Emit(_configuration.OutputPath, tsModel.Name, EmittedFileType.Model, contents);
                }
                else if (generatorEnum == Generator.Compiler)
                {
                    tsGenerator.GenerateDataModelAST(tsModel, _configuration.OutputPath);
                }
                
                log.Debug($"Class {tsModel.Name} emitted");
            }

            foreach (var tsModel in tsEnumModels)
            {
                if (generatorEnum == Generator.Template)
                {
                    var contents = modelGenerator.GenerateEnum(tsModel);
                    emitter.Emit(_configuration.OutputPath, tsModel.Name, EmittedFileType.Enum, contents);
                }
                else if (generatorEnum == Generator.Compiler)
                {
                    tsGenerator.GenerateEnumAST(tsModel, _configuration.OutputPath);
                }

                log.Debug($"Enum {tsModel.Name} emitted");
            }

            foreach (var tsModel in serviceModels)
            {
                if (generatorEnum == Generator.Template)
                {
                    var contents = webClientGenerator.GenerateService(tsModel);
                    emitter.Emit(_configuration.OutputPath, tsModel.Name, EmittedFileType.Service, contents);
                }
                else if (generatorEnum == Generator.Compiler)
                {
                    tsGenerator.GenerateServiceAST(tsModel, _configuration.OutputPath);
                }                

                log.Debug($"Service {tsModel.Name} emitted");
            }

            return Result.CreateSuccess();
        }
    }
}
