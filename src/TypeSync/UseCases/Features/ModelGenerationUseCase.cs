using System.IO;
using System.Linq;
using log4net;
using TypeSync.Common.Constants;
using TypeSync.Core.Features.ModelAnalysis;
using TypeSync.Models;
using TypeSync.Models.Converters;
using TypeSync.Output.Emitters;
using TypeSync.Output.Generators;

namespace TypeSync.UseCases
{
    public class ModelGenerationUseCase : IUseCase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ModelGenerationUseCase));

        private Configuration _configuration;

        public UseCase Id => UseCase.ModelGeneration;

        public string Description => "Generate TypeScript model classes from C# DTO objects.";

        public ModelGenerationUseCase(Configuration configuration)
        {
            _configuration = configuration;
        }

        public Result Handle()
        {
            var analyzer = new ModelAnalyzer(new ModelAnalysisContext());
            var converter = new ModelConverter();
            var generator = new ModelGenerator();
            var emitter = new TypeScriptEmitter();

            var analysisResult = analyzer.Analyze(_configuration.InputPath);

            if (!analysisResult.Success)
            {
                return Result.CreateError($"Source analysis error: {analysisResult.ErrorMessage}");
            }

            log.Debug("Source analyzed");

            var tsClassModels = converter.ConvertClasses(analysisResult.Value.Classes);
            var tsEnumModels = converter.ConvertEnums(analysisResult.Value.Enums);

            log.Debug("Models converted");

            foreach (var tsModel in tsClassModels)
            {
                var contents = generator.GenerateClass(tsModel);

                emitter.Emit(_configuration.OutputPath, tsModel.Name, EmittedFileType.Model, contents);

                log.Debug($"Class {tsModel.Name} emitted");

                new TsGenerator().GenerateDataModelAST(tsModel);
            }

            foreach (var tsModel in tsEnumModels)
            {
                var contents = generator.GenerateEnum(tsModel);

                emitter.Emit(_configuration.OutputPath, tsModel.Name, EmittedFileType.Enum, contents);

                log.Debug($"Enum {tsModel.Name} emitted");

                new TsGenerator().GenerateEnumAST(tsModel);
            }

            return Result.CreateSuccess();
        }
    }
}
