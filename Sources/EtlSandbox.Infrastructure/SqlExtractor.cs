using Dapper;

using EtlSandbox.Domain;
using EtlSandbox.Shared.Configurations;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EtlSandbox.Infrastructure;

public sealed class SqlExtractor : IExtractor<CustomerOrderFlat>
{
    private const int BatchSize = 100_000;
    private readonly ILogger<SqlExtractor> _logger;
    private readonly CustomerOrderFlatService _customerOrderFlatService;
    private readonly string _destinationConnectionString;

    public SqlExtractor(ILogger<SqlExtractor> logger, CustomerOrderFlatService customerOrderFlatService, IOptions<ConnectionStrings> options)
    {
        _logger = logger;
        _customerOrderFlatService = customerOrderFlatService;
        _destinationConnectionString = options.Value.SqlServer;
    }

    public async Task<IReadOnlyList<CustomerOrderFlat>> ExtractAsync(DateTime since, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Extracting data since {Since}", since);
        var result = await _customerOrderFlatService.GetCustomerOrderFlatsAsync(since);
        _logger.LogInformation("Extracted {Count} rows", result.Count);
        return result.ToList();
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
