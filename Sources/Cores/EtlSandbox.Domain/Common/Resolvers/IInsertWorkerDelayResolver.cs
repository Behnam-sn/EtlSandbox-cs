namespace EtlSandbox.Domain.Common.Resolvers;

public interface IInsertWorkerDelayResolver<TWorker, TSource, TDestination>
    where TWorker : class
    where TSource : class
    where TDestination : class, IEntity
{
    Task<int> GetDelayAsync();
}