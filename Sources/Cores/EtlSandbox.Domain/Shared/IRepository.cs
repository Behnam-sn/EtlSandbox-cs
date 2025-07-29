namespace EtlSandbox.Domain.Shared;

public interface IRepository<T>
    where T : class, IEntity
{
    Task<long> GetLastInsertedImportantIdAsync();

    Task<long> GetLastSoftDeletedItemIdAsync();

    Task<long> GetLastItemIdAsync();
}