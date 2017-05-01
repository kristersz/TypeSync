using log4net;
using TypeSync.Core.Features.ValidationAnalysis;
using TypeSync.Models;

namespace TypeSync.UseCases
{
    public class ValidatorGenerationUseCase : IUseCase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ValidatorGenerationUseCase));

        private Configuration _configuration;

        public UseCase Id => UseCase.ValidatatorGeneration;

        public string Description => "Generate TypeScript validators based on server side implemented .NET DTO validation.";

        public ValidatorGenerationUseCase(Configuration configuration)
        {
            _configuration = configuration;
        }

        public Result Handle()
        {
            var analyzer = new ValidationAnalyzer();

            var analysisResult = analyzer.Analyze(_configuration.InputPath);

            if (!analysisResult.Success)
            {
                return Result.CreateError($"Source analysis error: {analysisResult.ErrorMessage}");
            }

            log.Debug("Source analyzed");

            //var converter = new WebApiConverter();
            //var generator = new WebClientGenerator();
            //var emitter = new TypeScriptEmitter();

            //var serviceModels = converter.ConvertControllers(analysisResult.Value);

            //log.Debug("Models converted");

            //foreach (var tsModel in serviceModels)
            //{
            //    var contents = generator.GenerateService(tsModel);

            //    emitter.Emit(_configuration.OutputPath, tsModel.Name, EmittedFileType.Service, contents);

            //    log.Debug($"Service {tsModel.Name} emitted");
            //}

            return Result.CreateSuccess();
        }
    }
}
