namespace EtlSandbox.Domain.Shared;

public interface ILoader<T>
{
    Task LoadAsync(IEnumerable<T> data, CancellationToken cancellationToken);
}
