﻿using EtlSandbox.Domain.Shared;
using EtlSandbox.Domain.Shared.Options;
using EtlSandbox.Domain.Shared.Repositories;

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

    public async Task<long> GetLastSoftDeletedIdAsync(int batchSize)
    {
        using var scope = _serviceProvider.CreateScope();

        var destinationRepository = scope.ServiceProvider.GetRequiredService<IDestinationRepository<T>>();

        if (_lastSoftDeletedItemId == null)
        {
            _lastSoftDeletedItemId = await destinationRepository.GetLastSoftDeletedItemIdAsync();
            _startingPoint = _lastSoftDeletedItemId.Value;
            return _startingPoint;
        }

        var lastItemId = await destinationRepository.GetLastItemIdAsync();

        if (_startingPoint + batchSize < lastItemId)
        {
            _startingPoint += batchSize;
        }

        return _startingPoint;
    }
}