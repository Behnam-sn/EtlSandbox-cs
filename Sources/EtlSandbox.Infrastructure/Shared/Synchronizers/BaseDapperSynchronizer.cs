using Dapper;

using EtlSandbox.Domain.Shared;

namespace EtlSandbox.Infrastructure.Shared.Synchronizers;

public abstract class BaseDapperSynchronizer<T> : ISynchronizer<T>
    where T : class, IEntity
{
    private readonly IUnitOfWork _unitOfWork;

    protected BaseDapperSynchronizer(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    protected abstract string Sql { get; }

    public async Task SoftDeleteObsoleteRowsAsync(long fromId, long toId)
    {
        var parameters = new
        {
            FromId = fromId,
            ToId = toId,
        };

        using var connection = _unitOfWork.Connection;
        await connection.ExecuteAsync(Sql, parameters);
    }
}