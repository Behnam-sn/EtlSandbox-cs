using EtlSandbox.Domain.Shared;
using EtlSandbox.Infrastructure.Shared.Converters;

using Microsoft.Data.SqlClient;

namespace EtlSandbox.Infrastructure.Shared.Loaders;

public abstract class BaseSqlBulkCopyLoader<T> : ILoader<T>
    where T : class, IEntity
{
    private readonly IUnitOfWork _unitOfWork;

    protected BaseSqlBulkCopyLoader(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    protected abstract string TableName { get; }

    public async Task LoadAsync(List<T> items, CancellationToken cancellationToken)
    {
        var dataTable = DataTableConverter.ToDataTable(items);

        using var bulkCopy = new SqlBulkCopy(_unitOfWork.Connection.ConnectionString);
        bulkCopy.DestinationTableName = TableName;

        await bulkCopy.WriteToServerAsync(dataTable, cancellationToken);
    }
}