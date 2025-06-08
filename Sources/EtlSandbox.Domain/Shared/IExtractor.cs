namespace EtlSandbox.Domain.Shared;

public interface IExtractor<T>
{
    Task<IReadOnlyList<T>> ExtractAsync(int lastProcessedId, int batchSize, CancellationToken cancellationToken);
}