namespace EtlSandbox.Domain.Shared;

public interface ILoader<T>
    where T : class, IEntity
{
    Task LoadAsync(List<T> items, CancellationToken cancellationToken);
}
