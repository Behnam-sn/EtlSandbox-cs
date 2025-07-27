using EtlSandbox.Domain.Shared;
using EtlSandbox.Domain.Shared.Options;
using Microsoft.Extensions.Options;

namespace EtlSandbox.Infrastructure.Shared.Resolvers;

public class StartingPointResolver<T> : IStartingPointResolver<T> where T : class, IEntity
{
    private readonly IRepository<T> _repository;
    private readonly EntitySettings<T> _settings;

    public StartingPointResolver(IRepository<T> repository, IOptions<EntitySettings<T>> options)
    {
        _repository = repository;
        _settings = options.Value;
    }

    public async Task<long> GetLastProcessedIdAsync()
    {
        var dbLastProcessedId = await _repository.GetLastProcessedImportantIdAsync();
        var settingsLastProcessedId = _settings.LastProcessedId;
        
        return dbLastProcessedId < settingsLastProcessedId ? settingsLastProcessedId : dbLastProcessedId;
    }
}
