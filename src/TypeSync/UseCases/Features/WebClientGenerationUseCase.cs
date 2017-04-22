using System.IO;
using System.Linq;
using log4net;
using TypeSync.Common.Constants;
using TypeSync.Core.Features.WebApiAnalysis;
using TypeSync.Models;
using TypeSync.Models.Converters;
using TypeSync.Output.Emitters;
using TypeSync.Output.Generators;

namespace TypeSync.UseCases
{
    public class WebClientGenerationUseCase : IUseCase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WebClientGenerationUseCase));

        private Configuration _configuration;

        public UseCase Id => UseCase.WebClientGeneration;

        public string Description => "Generate Angular specific Typescript services that can consume ASP.NET Web APIs.";

        public WebClientGenerationUseCase(Configuration configuration)
        {
            _configuration = configuration;
        }

        public Result Handle()
        {
            var supportedExtensions = DotNetFileExtension.All;
            var extension = Path.GetExtension(_configuration.InputPath);

            if (!supportedExtensions.Contains(extension))
            {
                return Result.CreateError(string.Format("Unsupported path extension - {0}.  Supported extensions: {1}", extension, string.Join(", ", supportedExtensions)));
            }

            var analyzer = new WebApiAnalyzer();

            var analysisResult = analyzer.Analyze(_configuration.InputPath);

            if (!analysisResult.Success)
            {
                return Result.CreateError($"Source analysis error: {analysisResult.ErrorMessage}");
            }

            log.Debug("Source analyzed");

            var converter = new WebApiConverter();
            var generator = new WebClientGenerator();
            var emitter = new TypeScriptEmitter();

            var serviceModels = converter.ConvertControllers(analysisResult.Value);

            log.Debug("Models converted");

            foreach (var tsModel in serviceModels)
            {
                var contents = generator.GenerateService(tsModel);

                emitter.Emit(_configuration.OutputPath, tsModel.Name, EmittedFileType.Service, contents);

                log.Debug($"Service {tsModel.Name} emitted");
            }

            return Result.CreateSuccess();
        }
    }
}
