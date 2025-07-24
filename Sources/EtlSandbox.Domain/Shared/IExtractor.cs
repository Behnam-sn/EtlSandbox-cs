namespace EtlSandbox.Domain.Shared;

public interface IExtractor<T>
    where T : class, IEntity
{
    Task<List<T>> ExtractAsync(long lastProcessedId, int batchSize, CancellationToken cancellationToken);
}