namespace EtlSandbox.Domain.Shared.Repositories;

public interface ISourceRepository<T>
    where T : class
{
    Task<long> GetLastItemIdAsync();
}