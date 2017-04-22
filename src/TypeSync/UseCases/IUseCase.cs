using TypeSync.Models;

namespace TypeSync.UseCases
{
    public interface IUseCase
    {
        UseCase Id { get; }

        string Description { get; }

        Result Handle();
    }
}
