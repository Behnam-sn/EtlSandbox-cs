using EtlSandbox.Domain.Shared;
using EtlSandbox.Domain.Shared.Options;

using Microsoft.Extensions.Options;

namespace EtlSandbox.Infrastructure.Shared.Resolvers;

public class InsertStartingPointResolver<T> : IInsertStartingPointResolver<T> where T : class, IEntity
{
    private readonly IRepository<T> _repository;
    private readonly EntitySettings<T> _settings;

    public InsertStartingPointResolver(IRepository<T> repository, IOptions<EntitySettings<T>> options)
    {
        _repository = repository;
        _settings = options.Value;
    }

    public async Task<long> GetLastInsertedIdAsync()
    {
        var dbLastInsertedId = await _repository.GetLastInsertedImportantIdAsync();
        var settingsLastInsertedId = _settings.LastInsertedId;

        return dbLastInsertedId < settingsLastInsertedId ? settingsLastInsertedId : dbLastInsertedId;
    }
}
