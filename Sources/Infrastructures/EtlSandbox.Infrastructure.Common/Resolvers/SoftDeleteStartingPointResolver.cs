using EtlSandbox.Domain.Common;
using EtlSandbox.Domain.Common.Repositories;
using EtlSandbox.Domain.Common.Resolvers;

using Microsoft.Extensions.DependencyInjection;

namespace EtlSandbox.Infrastructure.Common.Resolvers;

public sealed class SoftDeleteStartingPointResolver<T> : ISoftDeleteStartingPointResolver<T>
    where T : class, IEntity
{
    private readonly IServiceProvider _serviceProvider;

    private long? _lastSoftDeletedId;

    private long _startingPoint;

    public SoftDeleteStartingPointResolver(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<long> GetLastSoftDeletedIdAsync(int batchSize)
    {
        using var scope = _serviceProvider.CreateScope();

        var destinationRepository = scope.ServiceProvider.GetRequiredService<IDestinationRepository<T>>();

        if (_lastSoftDeletedId == null)
        {
            _lastSoftDeletedId = await destinationRepository.GetLastSoftDeletedIdAsync();
            _startingPoint = _lastSoftDeletedId.Value;
            return _startingPoint;
        }

        var lastId = await destinationRepository.GetLastIdAsync();

        if (_startingPoint + batchSize < lastId)
        {
            _startingPoint += batchSize;
        }

        return _startingPoint;
    }
}