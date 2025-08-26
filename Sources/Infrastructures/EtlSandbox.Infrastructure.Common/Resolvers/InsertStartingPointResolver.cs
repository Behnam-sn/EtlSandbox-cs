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

    private bool _isFirstRun = true;

    // Todo: change this prop to a field
    public long StartingPoint { get; set; }

    public InsertStartingPointResolver(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<long> GetStartingPointAsync(long settingsStartingPoint)
    {
        using var scope = _serviceProvider.CreateScope();

        var destinationRepository = scope.ServiceProvider.GetRequiredService<IDestinationRepository<TDestination>>();

        if (_isFirstRun)
        {
            _isFirstRun = false;
            var lastInsertedSourceId = await destinationRepository.GetLastSourceIdAsync();
            StartingPoint = lastInsertedSourceId < settingsStartingPoint
                ? settingsStartingPoint
                : lastInsertedSourceId;
        }

        return StartingPoint;
    }

    public void SetStartingPoint(long startingPoint)
    {
        StartingPoint = startingPoint;
    }
}