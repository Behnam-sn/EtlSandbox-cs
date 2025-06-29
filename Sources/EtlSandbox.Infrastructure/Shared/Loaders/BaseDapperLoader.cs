using System.Data;

using Dapper;

using EtlSandbox.Domain.Shared;

using Microsoft.Extensions.Logging;

namespace EtlSandbox.Infrastructure.Shared.Loaders;

public abstract class BaseDapperLoader<T> : ILoader<T>
{
    private readonly ILogger<BaseDapperLoader<T>> _logger;

    private readonly IUnitOfWork _unitOfWork;

    private readonly string _sql;

    protected BaseDapperLoader(ILogger<BaseDapperLoader<T>> logger, IUnitOfWork unitOfWork, string sql)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _sql = sql;
    }

    public async Task LoadAsync(List<T> data, CancellationToken cancellationToken, IDbTransaction? transaction = null)
    {
        if (data.Count == 0)
        {
            _logger.LogInformation("No data to load");
            return;
        }

        if (transaction is null)
        {
            var connection = _unitOfWork.Connection;
            await connection.ExecuteAsync(_sql, data);
        }
        else
        {
            var connection = transaction.Connection ?? throw new ArgumentNullException(nameof(transaction), "Transaction must have a valid connection.");
            await connection.ExecuteAsync(_sql, data, transaction);
        }
    }
}