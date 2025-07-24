namespace EtlSandbox.Domain.Shared;

public interface IRepository<T>
    where T : class, IEntity
{
    Task<long> GetLastProcessedImportantIdAsync();

    Task<long> GetLastSoftDeletedItemIdAsync();

    Task<long> GetLastItemIdAsync();
}