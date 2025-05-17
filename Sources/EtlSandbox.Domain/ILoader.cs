namespace EtlSandbox.Domain;

public interface ILoader<T>
{
    Task LoadAsync(IEnumerable<T> data, CancellationToken cancellationToken);
}
