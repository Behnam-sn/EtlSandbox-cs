using EtlSandbox.Domain.Shared;
using EtlSandbox.Domain.Shared.Options;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EtlSandbox.Infrastructure.Shared.Resolvers;

public sealed class SoftDeleteStartingPointResolver<T> : ISoftDeleteStartingPointResolver<T>
    where T : class, IEntity
{
    private readonly IServiceProvider _serviceProvider;

    private long? _lastSoftDeletedItemId;

    private long _startingPoint;

    public SoftDeleteStartingPointResolver(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<long> GetLastSoftDeletedIdAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRepository<T>>();
        var applicationSettings = scope.ServiceProvider.GetRequiredService<IOptions<ApplicationSettings>>();

        if (_lastSoftDeletedItemId == null)
        {
            _lastSoftDeletedItemId = await repository.GetLastSoftDeletedItemIdAsync();
            _startingPoint = _lastSoftDeletedItemId.Value;
            return _startingPoint;
        }

        var lastItemId = await repository.GetLastItemIdAsync();
        var batchSize = applicationSettings.Value.BatchSize;

        if (_startingPoint + batchSize < lastItemId)
        {
            _startingPoint += batchSize;
        }

        return _startingPoint;
    }
}