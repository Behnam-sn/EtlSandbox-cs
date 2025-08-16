using EtlSandbox.Domain.Common.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace EtlSandbox.Infrastructure.Common.ConfigureOptions;

public sealed class ConnectionStringsSetup : IConfigureOptions<ConnectionStrings>
{
    private readonly IConfiguration _configuration;

    public ConnectionStringsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(ConnectionStrings options)
    {
        _configuration
            .GetSection("ConnectionStrings")
            .Bind(options);
    }
}