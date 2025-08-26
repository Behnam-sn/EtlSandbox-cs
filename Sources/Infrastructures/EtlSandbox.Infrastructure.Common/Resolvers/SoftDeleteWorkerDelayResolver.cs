using EtlSandbox.Domain.Common;
using EtlSandbox.Domain.Common.Options;
using EtlSandbox.Domain.Common.Options.WorkerSettings;
using EtlSandbox.Domain.Common.Repositories;
using EtlSandbox.Domain.Common.Resolvers;

using Microsoft.Extensions.Options;

namespace EtlSandbox.Infrastructure.Common.Resolvers;

public sealed class SoftDeleteWorkerDelayResolver<TWorker, TDestination> : ISoftDeleteWorkerDelayResolver<TWorker, TDestination>
    where TWorker : class
    where TDestination : class, IEntity
{
    private readonly GlobalSettings  _globalSettings;
    
    private readonly SoftDeleteWorkerSettings<TWorker>  _softDeleteWorkerSettings;
    
    private readonly ISoftDeleteStartingPointResolver<TDestination> _startingPointResolver;
    
    private readonly IDestinationRepository<TDestination> _destinationRepository;

    public SoftDeleteWorkerDelayResolver(
        IOptions<GlobalSettings> globalSettingsOptions,
        IOptions<SoftDeleteWorkerSettings<TWorker>> softDeleteWorkerSettingsOptions,
        ISoftDeleteStartingPointResolver<TDestination> startingPointResolver,
        IDestinationRepository<TDestination> destinationRepository
    )
    {
        _globalSettings = globalSettingsOptions.Value;
        _softDeleteWorkerSettings = softDeleteWorkerSettingsOptions.Value;
        _startingPointResolver = startingPointResolver;
        _destinationRepository = destinationRepository;
    }
    
    public async Task<int> GetDelayAsync()
    {
        var minDelay = _softDeleteWorkerSettings.MinDelayInMilliSeconds ?? _globalSettings.MinDelayInMilliSeconds;
        var maxDelay = _softDeleteWorkerSettings.MaxDelayInMilliSeconds ?? _globalSettings.MaxDelayInMilliSeconds;

        var minBatchSize = _softDeleteWorkerSettings.MinBatchSize ?? _globalSettings.MinBatchSize;

        var startingPoint = _startingPointResolver.StartingPoint;
        var lastId = await _destinationRepository.GetLastIdAsync();
        var gap = lastId - startingPoint;

        return gap < minBatchSize ? minDelay : maxDelay;
    }
}