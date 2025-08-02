namespace EtlSandbox.Domain.Shared;

public interface IInsertStartingPointResolver<TSource, TDestination>
    where TSource : class
    where TDestination : class, IEntity
{
    Task<long> GetStartingPointAsync(int batchSize);
}