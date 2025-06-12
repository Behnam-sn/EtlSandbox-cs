using System.Data;

namespace EtlSandbox.Domain.Shared;

public interface ISynchronizer<T>
{
    Task SoftDeleteObsoleteRowsAsync(int fromId, int toId, IDbTransaction? transaction = null);
}