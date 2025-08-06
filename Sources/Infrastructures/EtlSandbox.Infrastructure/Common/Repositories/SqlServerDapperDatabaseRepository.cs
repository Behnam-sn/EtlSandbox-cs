using Dapper;

using EtlSandbox.Domain.Common;
using EtlSandbox.Domain.Common.Repositories;

namespace EtlSandbox.Infrastructure.Common.Repositories;

public sealed class SqlServerDapperDatabaseRepository : IDatabaseRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public SqlServerDapperDatabaseRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
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

        using var connection = _dbConnectionFactory.CreateConnection();
        var columns = await connection.QueryAsync(sql, parameters);
        return columns.ToList();
    }
}