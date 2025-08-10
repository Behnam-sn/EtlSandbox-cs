using EtlSandbox.Domain.Common.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace EtlSandbox.Infrastructure.Common.ConfigureOptions;

public sealed class GlobalSettingsSetup : IConfigureOptions<GlobalSettings>
{
    private readonly IConfiguration _configuration;

    public GlobalSettingsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(GlobalSettings options)
    {
        _configuration
            .GetSection("GlobalSettings")
            .Bind(options);
    }
}