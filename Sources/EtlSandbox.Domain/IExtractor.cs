namespace EtlSandbox.Domain;

public interface IExtractor<T>
{
    Task<IReadOnlyList<T>> ExtractAsync(DateTime since, CancellationToken cancellationToken);
    Task<DateTime> GetLastProcessedTimestampAsync();
    Task UpdateLastProcessedTimestampAsync(DateTime timestamp);
}