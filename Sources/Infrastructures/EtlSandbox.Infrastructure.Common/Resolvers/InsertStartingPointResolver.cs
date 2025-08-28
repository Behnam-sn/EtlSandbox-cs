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

    private long _startingPoint;

    public InsertStartingPointResolver(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<long> GetStartingPointAsync(long defaultStartingPoint)
    {
        using var scope = _serviceProvider.CreateScope();

        var destinationRepository = scope.ServiceProvider.GetRequiredService<IDestinationRepository<TDestination>>();

        if (_isFirstRun)
        {
            _isFirstRun = false;
            var lastInsertedSourceId = await destinationRepository.GetMaxSourceIdOrDefaultAsync();
            _startingPoint = lastInsertedSourceId < defaultStartingPoint
                ? defaultStartingPoint
                : lastInsertedSourceId;
        }

        return _startingPoint;
    }

    public void SetStartingPoint(long startingPoint)
    {
        _startingPoint = startingPoint;
    }
}