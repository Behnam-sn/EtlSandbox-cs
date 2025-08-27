namespace EtlSandbox.Domain.Common.Repositories;

public interface IDestinationRepository<T>
    where T : class, IEntity
{
    Task<long> GetMaxSourceIdOrDefaultAsync(CancellationToken cancellationToken = default);

    Task<long> GetMaxIdOrDefaultAsync(CancellationToken cancellationToken = default);
}