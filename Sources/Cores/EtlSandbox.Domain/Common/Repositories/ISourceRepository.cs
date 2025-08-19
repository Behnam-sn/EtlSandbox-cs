namespace EtlSandbox.Domain.Common.Repositories;

public interface ISourceRepository<T>
    where T : class
{
    Task<long> GetLastIdAsync(CancellationToken cancellationToken = default);
}