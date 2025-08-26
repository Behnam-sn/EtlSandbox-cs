using EtlSandbox.Domain.Common;
using EtlSandbox.Domain.Common.Options;
using EtlSandbox.Domain.Common.Options.WorkerSettings;
using EtlSandbox.Domain.Common.Repositories;
using EtlSandbox.Domain.Common.Resolvers;

using Microsoft.Extensions.Options;

namespace EtlSandbox.Infrastructure.Common.Resolvers;

public sealed class SoftDeleteWorkerBatchSizeResolver<TWorker, TDestination> : ISoftDeleteWorkerBatchSizeResolver<TWorker, TDestination>
    where TWorker : class
    where TDestination : class, IEntity
{
    
    private readonly GlobalSettings  _globalSettings;
    
    private readonly SoftDeleteWorkerSettings<TWorker>  _softDeleteWorkerSettings;
    
    private readonly ISoftDeleteStartingPointResolver<TDestination> _startingPointResolver;
    
    private readonly IDestinationRepository<TDestination> _destinationRepository;
    
    
    public SoftDeleteWorkerBatchSizeResolver(
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
    
    public async Task<int> GetBatchSizeAsync()
    {
        var minBatchSize = _softDeleteWorkerSettings.MinBatchSize ?? _globalSettings.MinBatchSize;
        var maxBatchSize = _softDeleteWorkerSettings.MaxBatchSize  ?? _globalSettings.MaxBatchSize;

        var startingPoint = _startingPointResolver.StartingPoint;
        var lastId = await _destinationRepository.GetLastIdAsync();
        var gap = lastId - startingPoint;

        return gap < minBatchSize ? minBatchSize : maxBatchSize;
    }
}