namespace EtlSandbox.Domain.Shared;

public interface IRestApiClient
{
    Task<T?> GetAsync<T>(string baseUrl, string path, object? queryParams = null, CancellationToken cancellationToken = default);
}
