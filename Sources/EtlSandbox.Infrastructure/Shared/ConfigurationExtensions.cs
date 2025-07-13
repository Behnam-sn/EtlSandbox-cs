using Microsoft.Extensions.Configuration;

namespace EtlSandbox.Infrastructure.Shared;

public static class ConfigurationExtensions
{
    public static void AddSharedConfiguration(this IConfigurationBuilder config)
    {
        // config
        // .SetBasePath(Directory.GetCurrentDirectory())
        // .AddJsonFile("appsettings.json", optional: false)
        // .AddJsonFile("appsettings.Shared.json", optional: true);
        // .AddEnvironmentVariables()
        // .Build();
    }
}