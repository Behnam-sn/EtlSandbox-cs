namespace EtlSandbox.Domain.Common.Repositories;

public interface IDestinationRepository<T>
    where T : class, IEntity
{
    Task<long> GetLastInsertedSourceIdAsync();

    Task<long> GetLastSoftDeletedItemIdAsync();

    Task<long> GetLastItemIdAsync();
}