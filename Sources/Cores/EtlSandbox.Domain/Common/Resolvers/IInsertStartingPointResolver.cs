namespace EtlSandbox.Domain.Common.Resolvers;

public interface IInsertStartingPointResolver<TSource, TDestination>
    where TSource : class
    where TDestination : class, IEntity
{
    Task<long> GetStartingPointAsync(long defaultStartingPoint);

    void SetStartingPoint(long startingPoint);
}