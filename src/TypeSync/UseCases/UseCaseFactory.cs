using TypeSync.Models;

namespace TypeSync.UseCases
{
    public static class UseCaseFactory
    {
        public static IUseCase Create(UseCase useCase, Configuration configuration)
        {
            switch (useCase)
            {
                case UseCase.ModelGeneration: return new ModelGenerationUseCase(configuration);
                case UseCase.WebClientGeneration: return new WebClientGenerationUseCase(configuration);
                case UseCase.ValidatatorGeneration: return new ValidatorGenerationUseCase(configuration);
                //case "GenerateModels": return new ModelGenerationUseCase(configuration);
                default:
                    return null;
            }
        }
    }
}
