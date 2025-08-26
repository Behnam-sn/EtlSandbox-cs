namespace EtlSandbox.Domain.Common.Resolvers;

public interface ISoftDeleteStartingPointResolver<T>
    where T : class, IEntity
{
    public long StartingPoint { get; set; }
}