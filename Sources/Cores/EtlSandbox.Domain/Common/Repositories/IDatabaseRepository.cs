namespace EtlSandbox.Domain.Common.Repositories;

public interface IDatabaseRepository
{
    Task<List<dynamic>> GetSchemaInformationAsync(string tableName);
}