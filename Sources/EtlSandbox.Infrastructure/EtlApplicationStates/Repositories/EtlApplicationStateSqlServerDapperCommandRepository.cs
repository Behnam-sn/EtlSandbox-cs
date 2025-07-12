using System.Data;

using Dapper;

using EtlSandbox.Domain.EtlApplicationStates.Enums;
using EtlSandbox.Domain.EtlApplicationStates.Repositories;
using EtlSandbox.Domain.Shared;

namespace EtlSandbox.Infrastructure.EtlApplicationStates.Repositories;

public sealed class EtlApplicationStateSqlServerDapperCommandRepository : IEtlApplicationStateCommandRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public EtlApplicationStateSqlServerDapperCommandRepository(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<int> GetLastProcessedIdAsync<T>(ProcessType processType)
    {
        var connection = _unitOfWork.Connection;
        const string sql = "SELECT MAX(LastProcessedId) FROM EtlApplicationStates WHERE EntityType = @EntityType AND ProcessType = @ProcessType";
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
        const string selectSql = "SELECT COUNT(1) FROM EtlApplicationStates WHERE EntityType = @EntityType AND ProcessType = @ProcessType";
        const string insertSql = "INSERT INTO EtlApplicationStates (EntityType, ProcessType, LastProcessedId) VALUES (@EntityType, @ProcessType, @LastProcessedId)";
        const string updateSql = "UPDATE EtlApplicationStates SET LastProcessedId = @LastProcessedId WHERE EntityType = @EntityType AND ProcessType = @ProcessType";

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