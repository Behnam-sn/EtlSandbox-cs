namespace EtlSandbox.Domain.Common.Resolvers;

public interface ISoftDeleteWorkerBatchSizeResolver<TWorker, TDestination>
    where TWorker : class
    where TDestination : class, IEntity
{
    Task<int> GetBatchSizeAsync();
}