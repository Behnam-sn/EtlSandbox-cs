namespace EtlSandbox.Domain.Shared;

public interface IStartingPointResolver<T>
    where T : class, IEntity
{
    Task<long> GetLastProcessedIdAsync();
}
