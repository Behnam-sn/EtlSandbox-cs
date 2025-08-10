using EtlSandbox.Domain.Common;
using EtlSandbox.Domain.Common.Options;
using EtlSandbox.Domain.Common.Repositories;
using EtlSandbox.Domain.Common.Resolvers;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EtlSandbox.Infrastructure.Common.Resolvers;

public sealed class InsertStartingPointResolver<TSource, TDestination>
    : IInsertStartingPointResolver<TSource, TDestination>
    where TSource : class
    where TDestination : class, IEntity
{
    private readonly IServiceProvider _serviceProvider;

    private long? _lastInsertedItemImportantId;

    private long _startingPoint;

    public InsertStartingPointResolver(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<long> GetStartingPointAsync(long settingsStartingPoint, int batchSize)
    {
        using var scope = _serviceProvider.CreateScope();

        var sourceRepository = scope.ServiceProvider.GetRequiredService<ISourceRepository<TSource>>();
        var destinationRepository = scope.ServiceProvider.GetRequiredService<IDestinationRepository<TDestination>>();

        if (_lastInsertedItemImportantId == null)
        {
            _lastInsertedItemImportantId = await destinationRepository.GetLastInsertedImportantIdAsync();
            _startingPoint = _lastInsertedItemImportantId.Value < settingsStartingPoint
                ? settingsStartingPoint
                : _lastInsertedItemImportantId.Value;
            return _startingPoint;
        }

        var sourceLastItemId = await sourceRepository.GetLastItemIdAsync();

        if (_startingPoint + batchSize < sourceLastItemId)
        {
            _startingPoint += batchSize;
            return _startingPoint;
        }

        _startingPoint = await destinationRepository.GetLastInsertedImportantIdAsync();

        return _startingPoint;
    }
}