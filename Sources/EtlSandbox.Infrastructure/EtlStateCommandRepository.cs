using Dapper;

using EtlSandbox.Domain;
using EtlSandbox.Shared.Configurations;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EtlSandbox.Infrastructure;

public sealed class EtlStateCommandRepository : IEtlStateCommandRepository
{
    private readonly ILogger<EtlStateCommandRepository> _logger;

    private readonly string _destinationConnectionString;

    public EtlStateCommandRepository(ILogger<EtlStateCommandRepository> logger, IOptions<ConnectionStrings> options)
    {
        _logger = logger;
        _destinationConnectionString = options.Value.SqlServer;
    }

    public async Task<int> GetLastProcessedIdAsync()
    {
        await using var connection = new SqlConnection(_destinationConnectionString);
        var result = await connection.ExecuteScalarAsync<int?>(
            "SELECT MAX(LastProcessedId) FROM EtlStates"
        );
        return result ?? int.MinValue;
    }

    public async Task UpdateLastProcessedIdAsync(int lastProcessedId)
    {
        await using var connection = new SqlConnection(_destinationConnectionString);
        await connection.ExecuteAsync(
            "INSERT INTO EtlStates (LastProcessedId) VALUES (@LastProcessedId)", 
            new { LastProcessedId = lastProcessedId }
        );

        _logger.LogInformation("Updated last processed id to {LastProcessedId}", lastProcessedId);
    }
}