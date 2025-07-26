using EtlSandbox.Domain.Shared;
using EtlSandbox.Domain.Shared.Options;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EtlSandbox.Infrastructure.Shared.Synchronizers;

public sealed class SynchronizerUtils<T> : ISynchronizerUtils<T>
    where T : class, IEntity
{
    private readonly IServiceProvider _serviceProvider;

    private long _lastSoftDeletedItemId;

    public SynchronizerUtils(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<long> GetLastSoftDeletedIdAsync()
    {
        using var scope = _serviceProvider.CreateScope();

        var repository = scope.ServiceProvider.GetRequiredService<IRepository<T>>();
        var lastSoftDeletedItemId = await repository.GetLastSoftDeletedItemIdAsync();

        if (lastSoftDeletedItemId >= _lastSoftDeletedItemId)
        {
            _lastSoftDeletedItemId = lastSoftDeletedItemId;
        }
        else
        {
            var lastItemId = await repository.GetLastItemIdAsync();
            var applicationSettings = scope.ServiceProvider.GetRequiredService<IOptions<ApplicationSettings>>();
            var batchSize = applicationSettings.Value.BatchSize;

            var temp = lastSoftDeletedItemId + batchSize;

            if (lastItemId > temp)
            {
                _lastSoftDeletedItemId = temp;
            }
        }

        return _lastSoftDeletedItemId;
    }
}