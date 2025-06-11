using Dapper;

using EtlSandbox.Domain.ApplicationStates;
using EtlSandbox.Domain.Shared.Options;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace EtlSandbox.Infrastructure.ApplicationStates;

public sealed class ApplicationStateDapperCommandRepository : IApplicationStateCommandRepository
{
    private readonly string _destinationDatabaseConnectionString;

    public ApplicationStateDapperCommandRepository(IOptions<DatabaseConnections> options)
    {
        _destinationDatabaseConnectionString = options.Value.SqlServer;
    }

    public async Task<int> GetLastProcessedIdAsync<T>(ActionType actionType)
    {
        await using var connection = new SqlConnection(_destinationDatabaseConnectionString);
        const string sql = "SELECT MAX(LastProcessedId) FROM ApplicationStates WHERE EntityType = @EntityType AND ActionType = @ActionType";
        var result = await connection.QuerySingleOrDefaultAsync<int?>(sql, new
        {
            EntityType = typeof(T).Name,
            ActionType = actionType
        });
        return result ?? 0;
    }

    public async Task UpdateLastProcessedIdAsync<T>(ActionType actionType, int lastProcessedId)
    {
        await using var connection = new SqlConnection(_destinationDatabaseConnectionString);
        var entityType = typeof(T).Name;
        const string selectSql = "SELECT COUNT(1) FROM ApplicationStates WHERE EntityType = @EntityType AND ActionType = @ActionType";
        var exists = await connection.ExecuteScalarAsync<int>(selectSql, new
        {
            EntityType = entityType,
            ActionType = actionType
        });

        if (exists == 0)
        {
            const string insertSql = "INSERT INTO ApplicationStates (EntityType, ActionType, LastProcessedId) VALUES (@EntityType, @ActionType, @LastProcessedId)";
            await connection.ExecuteAsync(insertSql, new
            {
                EntityType = entityType,
                ActionType = actionType,
                LastProcessedId = lastProcessedId
            });
        }
        else
        {
            const string updateSql = "UPDATE ApplicationStates SET LastProcessedId = @LastProcessedId WHERE EntityType = @EntityType AND ActionType = @ActionType";
            await connection.ExecuteAsync(updateSql, new
            {
                EntityType = entityType,
                ActionType = actionType,
                LastProcessedId = lastProcessedId
            });
        }
    }
}