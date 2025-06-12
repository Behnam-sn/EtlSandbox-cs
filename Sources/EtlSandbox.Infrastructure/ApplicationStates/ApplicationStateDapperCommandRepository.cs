using System.Data;

using Dapper;

using EtlSandbox.Domain.ApplicationStates;
using EtlSandbox.Domain.ApplicationStates.Enums;
using EtlSandbox.Domain.ApplicationStates.Repositories;
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

    public async Task UpdateLastProcessedIdAsync<T>(ActionType actionType, int lastProcessedId, IDbTransaction? transaction = null)
    {
        var entityType = typeof(T).Name;
        const string selectSql = "SELECT COUNT(1) FROM ApplicationStates WHERE EntityType = @EntityType AND ActionType = @ActionType";
        const string insertSql = "INSERT INTO ApplicationStates (EntityType, ActionType, LastProcessedId) VALUES (@EntityType, @ActionType, @LastProcessedId)";
        const string updateSql = "UPDATE ApplicationStates SET LastProcessedId = @LastProcessedId WHERE EntityType = @EntityType AND ActionType = @ActionType";

        var parameters = new
        {
            EntityType = entityType,
            ActionType = actionType,
            LastProcessedId = lastProcessedId
        };

        if (transaction is null)
        {
            await using var connection = new SqlConnection(_destinationDatabaseConnectionString);
            var exists = await connection.ExecuteScalarAsync<int>(selectSql, parameters);
            if (exists == 0)
            {
                await connection.ExecuteAsync(insertSql, parameters);
            }
            else
            {
                await connection.ExecuteAsync(updateSql, parameters);
            }
        }
        else
        {
            var connection = transaction.Connection ?? throw new ArgumentNullException(nameof(transaction), "Transaction must have a valid connection.");
            var exists = await connection.ExecuteScalarAsync<int>(selectSql, parameters, transaction);
            if (exists == 0)
            {
                await connection.ExecuteAsync(insertSql, parameters, transaction);
            }
            else
            {
                await connection.ExecuteAsync(updateSql, parameters, transaction);
            }
        }
    }
}