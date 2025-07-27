namespace EtlSandbox.Domain.Shared;

public interface IInsertStartingPointResolver<T>
    where T : class, IEntity
{
    Task<long> GetLastProcessedIdAsync();
}
