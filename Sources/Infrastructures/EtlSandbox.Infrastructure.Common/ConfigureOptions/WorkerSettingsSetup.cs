using EtlSandbox.Domain.Common.Options.WorkerSettings;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace EtlSandbox.Infrastructure.Common.ConfigureOptions;

public sealed class WorkerSettingsSetup<T> : IConfigureOptions<WorkerSettings<T>>
    where T : BackgroundService
{
    private readonly IConfiguration _configuration;

    public WorkerSettingsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(WorkerSettings<T> options)
    {
        _configuration
            .GetSection(typeof(T).Name + "Settings")
            .Bind(options);
    }
}