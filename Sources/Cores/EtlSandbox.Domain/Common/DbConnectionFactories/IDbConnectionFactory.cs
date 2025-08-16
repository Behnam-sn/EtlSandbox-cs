using System.Data;

namespace EtlSandbox.Domain.Common.DbConnectionFactories;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
