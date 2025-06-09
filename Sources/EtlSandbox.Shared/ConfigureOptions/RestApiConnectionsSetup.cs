using EtlSandbox.Domain.Shared.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace EtlSandbox.Shared.ConfigureOptions;

public sealed class RestApiConnectionsSetup : IConfigureOptions<RestApiConnections>
{
    private const string SectionName = "RestApiConnections";

    private readonly IConfiguration _configuration;

    public RestApiConnectionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(RestApiConnections options)
    {
        _configuration
            .GetSection(SectionName)
            .Bind(options);
    }
}