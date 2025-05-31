using Microsoft.Extensions.Configuration;

namespace EtlSandbox.Shared;

public static class ConfigurationExtensions
{
    public static IConfiguration AddSharedConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("../Shared/appsettings.Shared.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
    }
}