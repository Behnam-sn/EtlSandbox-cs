namespace EtlSandbox.Domain;

public interface IExtractor<T>
{
    Task<IReadOnlyList<T>> ExtractAsync(int lastProcessedId, CancellationToken cancellationToken);
}
