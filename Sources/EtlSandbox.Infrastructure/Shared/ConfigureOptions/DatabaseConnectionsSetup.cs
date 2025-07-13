using EtlSandbox.Domain.Shared.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace EtlSandbox.Infrastructure.Shared.ConfigureOptions;

public sealed class DatabaseConnectionsSetup : IConfigureOptions<DatabaseConnections>
{
    private const string SectionName = "DatabaseConnections";

    private readonly IConfiguration _configuration;

    public DatabaseConnectionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(DatabaseConnections options)
    {
        _configuration
            .GetSection(SectionName)
            .Bind(options);
    }
}
