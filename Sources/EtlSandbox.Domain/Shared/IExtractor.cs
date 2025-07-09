namespace EtlSandbox.Domain.Shared;

public interface IExtractor<T>
{
    Task<List<T>> ExtractAsync(int lastProcessedId, int batchSize, CancellationToken cancellationToken);
}