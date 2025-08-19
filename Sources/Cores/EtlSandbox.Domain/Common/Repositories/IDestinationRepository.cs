namespace EtlSandbox.Domain.Common.Repositories;

public interface IDestinationRepository<T>
    where T : class, IEntity
{
    Task<long> GetLastSourceIdAsync();

    Task<long> GetLastSoftDeletedIdAsync();

    Task<long> GetLastIdAsync();
}