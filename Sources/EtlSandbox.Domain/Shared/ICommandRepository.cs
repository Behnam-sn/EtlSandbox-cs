namespace EtlSandbox.Domain.Shared;

public interface ICommandRepository<T>
{
    Task<List<int>> GetIdsAsync(int from, int to);
}