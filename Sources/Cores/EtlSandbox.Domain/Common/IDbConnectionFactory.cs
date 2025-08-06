using System.Data;

namespace EtlSandbox.Domain.Common;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}