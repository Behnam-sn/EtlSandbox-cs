using System.Data;

namespace EtlSandbox.Domain.Shared;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}