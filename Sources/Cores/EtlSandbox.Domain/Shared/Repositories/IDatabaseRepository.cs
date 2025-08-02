namespace EtlSandbox.Domain.Shared.Repositories;

public interface IDatabaseRepository
{
    Task<List<dynamic>> GetSchemaInformationAsync(string tableName);
}