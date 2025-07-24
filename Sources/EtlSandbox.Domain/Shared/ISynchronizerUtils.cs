namespace EtlSandbox.Domain.Shared;

public interface ISynchronizerUtils<T>
    where T : class, IEntity
{
    Task<long> GetLastSoftDeletedIdAsync();
}