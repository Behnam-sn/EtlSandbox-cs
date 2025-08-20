namespace EtlSandbox.Domain.Common.Resolvers;

public interface IInsertStartingPointResolver<TSource, TDestination>
    where TSource : class
    where TDestination : class, IEntity
{
    Task<long> GetStartingPointAsync(long settingsStartingPoint);

    void SetStartingPoint(long startingPoint);
}