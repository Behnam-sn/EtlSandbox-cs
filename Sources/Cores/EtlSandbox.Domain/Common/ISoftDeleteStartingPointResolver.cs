namespace EtlSandbox.Domain.Common;

public interface ISoftDeleteStartingPointResolver<T>
    where T : class, IEntity
{
    Task<long> GetLastSoftDeletedIdAsync(int batchSize);
}