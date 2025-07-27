using EtlSandbox.Domain.Shared;
using EtlSandbox.Domain.Shared.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace EtlSandbox.Infrastructure.Shared.ConfigureOptions;

public sealed class EntitySettingsSetup<T> : IConfigureOptions<EntitySettings<T>>
    where T : class, IEntity
{
    private readonly IConfiguration _configuration;

    public EntitySettingsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(EntitySettings<T> options)
    {
        _configuration
            .GetSection(typeof(T).Name + "Settings")
            .Bind(options);
    }
}