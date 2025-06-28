using System.Data;

using Dapper;

using EtlSandbox.Domain.ApplicationStates.Enums;
using EtlSandbox.Domain.ApplicationStates.Repositories;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Domain.Shared.Options;

using Microsoft.Extensions.Options;

using Npgsql;

namespace EtlSandbox.Infrastructure.ApplicationStates;

public sealed class ApplicationStatePostgreSqlDapperCommandRepository : IApplicationStateCommandRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public ApplicationStatePostgreSqlDapperCommandRepository(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<int> GetLastProcessedIdAsync<T>(ProcessType processType)
    {
        var connection = _unitOfWork.Connection;
        const string sql = "SELECT MAX(\"LastProcessedId\") FROM \"ApplicationStates\" WHERE \"EntityType\" = @EntityType AND \"ProcessType\" = @ProcessType";
        var result = await connection.QuerySingleOrDefaultAsync<int?>(sql, new
        {
            EntityType = typeof(T).Name,
            ProcessType = processType
        });
        return result ?? 0;
    }

    public async Task UpdateLastProcessedIdAsync<T>(ProcessType processType, int lastProcessedId, IDbTransaction? transaction = null)
    {
        var entityType = typeof(T).Name;
        const string selectSql = "SELECT COUNT(1) FROM \"ApplicationStates\" WHERE \"EntityType\" = @EntityType AND \"ProcessType\" = @ProcessType";
        const string insertSql = "INSERT INTO \"ApplicationStates\" (\"EntityType\", \"ProcessType\", \"LastProcessedId\") VALUES (@EntityType, @ProcessType, @LastProcessedId)";
        const string updateSql = "UPDATE \"ApplicationStates\" SET \"LastProcessedId\" = @LastProcessedId WHERE \"EntityType\" = @EntityType AND \"ProcessType\" = @ProcessType";

        var parameters = new
        {
            EntityType = entityType,
            ProcessType = processType,
            LastProcessedId = lastProcessedId
        };

        if (transaction is null)
        {
            var connection = _unitOfWork.Connection;
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