namespace EtlSandbox.Domain.Shared;

public interface ISoftDeleteStartingPointResolver<T>
    where T : class, IEntity
{
    Task<long> GetLastSoftDeletedIdAsync(int batchSize);
}