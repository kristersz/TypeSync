using TypeSync.Models;

namespace TypeSync.UseCases
{
    public interface IUseCase
    {
        string Id { get; }

        string Name { get; }

        Result Handle();
    }
}
