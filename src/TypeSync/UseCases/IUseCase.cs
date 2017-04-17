using TypeSync.Models;

namespace TypeSync.UseCases
{
    public interface IUseCase
    {
        string Id { get; }

        string Description { get; }

        Result Handle();
    }
}
