using Dapper;

using EtlSandbox.Domain.Shared;

namespace EtlSandbox.Infrastructure.Shared.Loaders;

public abstract class BaseDapperLoader<T> : ILoader<T>
    where T : class, IEntity
{
    private readonly IUnitOfWork _unitOfWork;

    protected BaseDapperLoader(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    protected abstract string Sql { get; }

    public async Task LoadAsync(List<T> items, CancellationToken cancellationToken)
    {
        if (items.Count == 0)
        {
            return;
        }

        using var connection = _unitOfWork.Connection;
        await connection.ExecuteAsync(Sql, items);
    }
}