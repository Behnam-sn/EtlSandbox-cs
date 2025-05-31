
using System.Net.Http.Json;

using Dapper;

using EtlSandbox.Domain;
using EtlSandbox.Shared.Configurations;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EtlSandbox.Infrastructure;

public sealed class RestExtractor : IExtractor<CustomerOrderFlat>
{
    private readonly ILogger<RestExtractor> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _destinationConnectionString;

    public RestExtractor(ILogger<RestExtractor> logger, IHttpClientFactory httpClientFactory, IOptions<ConnectionStrings> options)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
        _destinationConnectionString = options.Value.SqlServer;
    }

    public async Task<IReadOnlyList<CustomerOrderFlat>> ExtractAsync(DateTime since, CancellationToken cancellationToken)
    {
        var url = "http://localhost:5050/api/customers";
        url += $"?since={since:o}";

        var customers = await _httpClient.GetFromJsonAsync<List<CustomerOrderFlat>>(url);
        return customers ?? [];
    }

    public async Task<DateTime> GetLastProcessedTimestampAsync()
    {
        using var connection = new SqlConnection(_destinationConnectionString);
        var result = await connection.ExecuteScalarAsync<DateTime?>(
            "SELECT MAX(LastProcessedAt) FROM EtlStates"
        );
        return result ?? DateTime.MinValue;
    }

    public async Task UpdateLastProcessedTimestampAsync(DateTime timestamp)
    {
        using var connection = new SqlConnection(_destinationConnectionString);
        await connection.ExecuteAsync("INSERT INTO EtlStates (LastProcessedAt) VALUES (@Timestamp)", new { Timestamp = timestamp });

        _logger.LogInformation("Updated last processed timestamp to {Timestamp}", timestamp);
    }
}
