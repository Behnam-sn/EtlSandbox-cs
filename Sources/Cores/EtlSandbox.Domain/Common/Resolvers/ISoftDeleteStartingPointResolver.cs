namespace EtlSandbox.Domain.Common.Resolvers;

public interface ISoftDeleteStartingPointResolver<T>
    where T : class, IEntity
{
    Task<long> GetLastSoftDeletedIdAsync(int batchSize);
}