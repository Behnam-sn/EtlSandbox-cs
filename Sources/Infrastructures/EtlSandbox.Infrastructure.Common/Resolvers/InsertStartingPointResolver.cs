using EtlSandbox.Domain.Common;
using EtlSandbox.Domain.Common.Repositories;
using EtlSandbox.Domain.Common.Resolvers;

using Microsoft.Extensions.DependencyInjection;

namespace EtlSandbox.Infrastructure.Common.Resolvers;

public sealed class InsertStartingPointResolver<TSource, TDestination>
    : IInsertStartingPointResolver<TSource, TDestination>
    where TSource : class
    where TDestination : class, IEntity
{
    private readonly IServiceProvider _serviceProvider;

    private long? _lastInsertedItemSourceId;

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

        if (_lastInsertedItemSourceId == null)
        {
            _lastInsertedItemSourceId = await destinationRepository.GetLastSourceIdAsync();
            _startingPoint = _lastInsertedItemSourceId.Value < settingsStartingPoint
                ? settingsStartingPoint
                : _lastInsertedItemSourceId.Value;
            return _startingPoint;
        }

        var sourceLastId = await sourceRepository.GetLastIdAsync();

        if (_startingPoint + batchSize < sourceLastId)
        {
            _startingPoint += batchSize;
            return _startingPoint;
        }

        _startingPoint = await destinationRepository.GetLastSourceIdAsync();

        return _startingPoint;
    }
}