using log4net;

namespace TypeSync.UseCases
{
    public class ValidatorGenerationUseCase : IUseCase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ValidatorGenerationUseCase));

        public string Id { get; } = "GenerateValidators";

        public string Name { get; } = "Generate TypeScript validators based on server side implemented .NET DTO validation.";

        public ValidatorGenerationUseCase()
        {
        }

        public void Execute()
        {
        }
    }
}
