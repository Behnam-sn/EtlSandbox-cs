using System.Data;

using Dapper;

using EtlSandbox.Domain.Shared;

namespace EtlSandbox.Infrastructure.Shared.Synchronizers;

public abstract class BaseDapperSynchronizer<T> : ISynchronizer<T>
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly string _sql;

    protected BaseDapperSynchronizer(IUnitOfWork unitOfWork, string sql)
    {
        _unitOfWork = unitOfWork;
        _sql = sql;
    }

    public async Task SoftDeleteObsoleteRowsAsync(int fromId, int toId, IDbTransaction? transaction = null)
    {
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