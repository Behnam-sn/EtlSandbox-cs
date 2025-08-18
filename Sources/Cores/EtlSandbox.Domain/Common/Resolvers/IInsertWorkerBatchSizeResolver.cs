namespace EtlSandbox.Domain.Common.Resolvers;

public interface IInsertWorkerBatchSizeResolver<TWorker, TSource, TDestination>
    where TWorker : class
    where TSource : class
    where TDestination : class, IEntity
{
    Task<int> GetBatchSizeAsync();
}