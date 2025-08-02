namespace EtlSandbox.Domain.Shared;

public interface IExtractor<T>
    where T : class, IEntity
{
    Task<List<T>> ExtractAsync(long from, long to, CancellationToken cancellationToken = default);
}