using EtlSandbox.Domain.Shared.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace EtlSandbox.Shared.ConfigureOptions;

public sealed class ApplicationSettingsSetup : IConfigureOptions<ApplicationSettings>
{
    private const string SectionName = "ApplicationSettings";

    private readonly IConfiguration _configuration;

    public ApplicationSettingsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(ApplicationSettings options)
    {
        _configuration
            .GetSection(SectionName)
            .Bind(options);
    }
}
