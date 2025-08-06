using EtlSandbox.Domain.Common.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace EtlSandbox.Infrastructure.Common.ConfigureOptions;

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
