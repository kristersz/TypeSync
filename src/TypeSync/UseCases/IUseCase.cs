namespace TypeSync.UseCases
{
    public interface IUseCase
    {
        string Id { get; }

        string Name { get; }

        void Execute();
    }
}
