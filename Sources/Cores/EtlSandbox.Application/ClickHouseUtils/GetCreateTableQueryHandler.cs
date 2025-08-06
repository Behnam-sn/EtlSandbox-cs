using System.Text;

using EtlSandbox.Application.Shared.Abstractions.Messaging;
using EtlSandbox.Domain.Shared.Repositories;

namespace EtlSandbox.Application.ClickHouseUtils;

public sealed class GetCreateTableQueryHandler : IQueryHandler<GetCreateTableQuery, string>
{
    private readonly IDatabaseRepository _databaseRepository;

    public GetCreateTableQueryHandler(IDatabaseRepository databaseRepository)
    {
        _databaseRepository = databaseRepository;
    }

    public async Task<string> Handle(GetCreateTableQuery request, CancellationToken cancellationToken)
    {
        var columns = await _databaseRepository.GetSchemaInformationAsync(request.TableName);

        var sb = new StringBuilder();
        sb.AppendLine($"CREATE TABLE {request.TableName} (");

        foreach (var col in columns)
        {
            string colName = col.COLUMN_NAME;
            string sqlType = col.DATA_TYPE.ToString();
            string isNullable = col.IS_NULLABLE.ToString();
            string clickhouseType = MapToClickHouseType(sqlType, isNullable);

            sb.AppendLine($"    {colName} {clickhouseType},");
        }

        sb.Length -= 3; // Remove the last comma
        sb.AppendLine();
        sb.AppendLine(") ENGINE = MergeTree()");
        sb.AppendLine($"ORDER BY ({columns.First().COLUMN_NAME});");
        
        return sb.ToString();
    }

    private static string MapToClickHouseType(string sqlType, string isNullable)
    {
        string type = sqlType.ToLower() switch
        {
            "int" => "Int32",
            "bigint" => "Int64",
            "datetime" => "DateTime",
            "date" => "Date",
            "decimal" => "Decimal(10,2)",
            "nvarchar" => "String",
            "varchar" => "String",
            "bit" => "UInt8",
            "float" => "Float64",
            _ => "String" // Fallback
        };

        if (isNullable == "YES")
            type = $"Nullable({type})";

        return type;
    }
}