namespace EtlSandbox.Domain.Common.Resolvers;

public interface ISoftDeleteWorkerDelayResolver<TWorker, TDestination>
    where TWorker : class
    where TDestination : class, IEntity
{
    Task<int> GetDelayAsync();
}