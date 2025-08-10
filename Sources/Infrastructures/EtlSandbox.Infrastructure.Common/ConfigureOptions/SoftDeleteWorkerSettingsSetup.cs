using EtlSandbox.Domain.Common.Options.WorkerSettings;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace EtlSandbox.Infrastructure.Common.ConfigureOptions;

public sealed class SoftDeleteWorkerSettingsSetup<T> : IConfigureOptions<SoftDeleteWorkerSettings<T>>
    where T : BackgroundService
{
    private readonly IConfiguration _configuration;

    public SoftDeleteWorkerSettingsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(SoftDeleteWorkerSettings<T> options)
    {
        _configuration
            .GetSection(typeof(T).Name + "Settings")
            .Bind(options);
    }
}