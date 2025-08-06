namespace EtlSandbox.Domain.Common;

public interface ISynchronizer<T>
    where T : class, IEntity
{
    Task SoftDeleteObsoleteRowsAsync(long fromId, long toId);
}