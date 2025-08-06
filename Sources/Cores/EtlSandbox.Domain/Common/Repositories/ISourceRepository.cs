namespace EtlSandbox.Domain.Common.Repositories;

public interface ISourceRepository<T>
    where T : class
{
    Task<long> GetLastItemIdAsync(CancellationToken cancellationToken = default);
}