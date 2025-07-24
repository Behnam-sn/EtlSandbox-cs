using Dapper;

using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Domain.Shared;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Repositories;

public sealed class CustomerOrderFlatClickHouseDapperRepository : IRepository<CustomerOrderFlat>
{
    private readonly IUnitOfWork _unitOfWork;

    public CustomerOrderFlatClickHouseDapperRepository(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    private static string Table => "SakilaFlat.CustomerOrderFlats";

    public async Task<long> GetLastProcessedImportantIdAsync()
    {
        var sql = $"SELECT max(Id) FROM {Table}";
        using var connection = _unitOfWork.Connection;
        var result = await connection.QuerySingleOrDefaultAsync<long?>(sql);
        return result ?? 0;
    }

    public async Task<long> GetLastSoftDeletedItemIdAsync()
    {
        var sql = $"""
                   SELECT max(Id)
                   FROM {Table}
                   WHERE IsDeleted = 1;
                   """;
        using var connection = _unitOfWork.Connection;
        var result = await connection.QuerySingleOrDefaultAsync<long?>(sql);
        return result ?? 0;
    }

    public async Task<long> GetLastItemIdAsync()
    {
        var sql = $"SELECT max(Id) FROM {Table}";
        using var connection = _unitOfWork.Connection;
        var result = await connection.QuerySingleOrDefaultAsync<long?>(sql);
        return result ?? 0;
    }
}