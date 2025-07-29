namespace EtlSandbox.Domain.Shared;

public interface IDatabaseRepository
{
    Task<List<dynamic>> GetSchemaInformationAsync(string tableName);
}