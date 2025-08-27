namespace EtlSandbox.Domain.Common.Repositories;

public interface ISourceRepository<T>
    where T : class
{
    Task<long> GetMaxIdOrDefaultAsync(CancellationToken cancellationToken = default);
}