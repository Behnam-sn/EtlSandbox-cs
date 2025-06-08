using System.Threading;
using System.Threading.Tasks;

namespace EtlSandbox.Infrastructure.Shared.ApiClient;

public interface IApiClient
{
    Task<T?> GetAsync<T>(string baseUrl, string path, object? queryParams = null, CancellationToken cancellationToken = default);
    // You can add more methods like PostAsync, PutAsync, DeleteAsync as needed
}
