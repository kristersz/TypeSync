using log4net;
using TypeSync.Models;

namespace TypeSync.UseCases
{
    public class WebClientGenerationUseCase : IUseCase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WebClientGenerationUseCase));

        public string Id { get; } = "GenerateWebClient";

        public string Name { get; } = "Generate Angular specific Typescript services that can consume ASP.NET Web APIs.";

        public WebClientGenerationUseCase()
        {
        }

        public Result Handle()
        {
            return Result.CreateSuccess();
        }
    }
}
