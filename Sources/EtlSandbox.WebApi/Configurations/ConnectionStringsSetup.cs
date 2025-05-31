using Microsoft.Extensions.Options;

namespace EtlSandbox.WebApi.Configurations;

public sealed class ConnectionStringsSetup : IConfigureOptions<ConnectionStrings>
{
    private const string SectionName = "ConnectionStrings";
    private readonly IConfiguration _configuration;

    public ConnectionStringsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(ConnectionStrings options)
    {
        _configuration
            .GetSection(SectionName)
            .Bind(options);
    }
}