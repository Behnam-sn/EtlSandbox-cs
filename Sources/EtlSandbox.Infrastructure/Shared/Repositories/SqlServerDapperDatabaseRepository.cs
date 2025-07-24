using Dapper;

using EtlSandbox.Domain.Shared;

namespace EtlSandbox.Infrastructure.Shared.Repositories;

public sealed class SqlServerDapperDatabaseRepository : IDatabaseRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public SqlServerDapperDatabaseRepository(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<dynamic>> GetSchemaInformationAsync(string tableName)
    {
        const string sql = """
                               SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE
                               FROM INFORMATION_SCHEMA.COLUMNS
                               WHERE TABLE_NAME = @TableName
                               ORDER BY ORDINAL_POSITION
                           """;
        var parameters = new
        {
            TableName = tableName
        };

        using var connection = _unitOfWork.Connection;
        var columns = await connection.QueryAsync(sql, parameters);
        return columns.ToList();
    }
}