using System.Data;

using Dapper;

using EtlSandbox.Domain.Shared;

using Microsoft.Extensions.Logging;

namespace EtlSandbox.Infrastructure.Shared.Synchronizers;

public abstract class BaseDapperSynchronizer<T> : ISynchronizer<T>
{
    private readonly ILogger<BaseDapperSynchronizer<T>> _logger;

    private readonly IUnitOfWork _unitOfWork;

    private readonly string _sql;

    protected BaseDapperSynchronizer(ILogger<BaseDapperSynchronizer<T>> logger, IUnitOfWork unitOfWork, string sql)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _sql = sql;
    }

    public async Task SoftDeleteObsoleteRowsAsync(int fromId, int toId, IDbTransaction? transaction = null)
    {
        _logger.LogInformation("Deleting from {FromId} to {ToId}", fromId, toId);
        
        var parameters = new
        {
            FromId = fromId,
            ToId = toId,
        };

        if (transaction is null)
        {
            var connection = _unitOfWork.Connection;
            await connection.ExecuteAsync(_sql, parameters);
        }
        else
        {
            var connection = transaction.Connection ?? throw new ArgumentNullException(nameof(transaction), "Transaction must have a valid connection.");
            await connection.ExecuteAsync(_sql, parameters, transaction);
        }
    }
}