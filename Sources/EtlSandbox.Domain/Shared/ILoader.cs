using System.Data;

namespace EtlSandbox.Domain.Shared;

public interface ILoader<T>
{
    Task LoadAsync(List<T> data, CancellationToken cancellationToken, IDbTransaction? transaction = null);
}
