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
    
    private readonly WorkerSettings<TWorker>  _workerSettings;
    
    private readonly ISoftDeleteStartingPointResolver<TDestination> _startingPointResolver;
    
    private readonly IDestinationRepository<TDestination> _destinationRepository;
    
    
    public SoftDeleteWorkerBatchSizeResolver(
        IOptions<GlobalSettings> globalSettingsOptions,
        IOptions<WorkerSettings<TWorker>> workerSettingsOptions,
        ISoftDeleteStartingPointResolver<TDestination> startingPointResolver,
        IDestinationRepository<TDestination> destinationRepository
    )
    {
        _globalSettings = globalSettingsOptions.Value;
        _workerSettings = workerSettingsOptions.Value;
        _startingPointResolver = startingPointResolver;
        _destinationRepository = destinationRepository;
    }
    
    public async Task<int> GetBatchSizeAsync()
    {
        var minBatchSize = _workerSettings.MinBatchSize ?? _globalSettings.MinBatchSize;
        var maxBatchSize = _workerSettings.MaxBatchSize  ?? _globalSettings.MaxBatchSize;

        var startingPoint = _startingPointResolver.StartingPoint;
        var lastId = await _destinationRepository.GetMaxIdOrDefaultAsync();
        var gap = lastId - startingPoint;

        return gap < minBatchSize ? minBatchSize : maxBatchSize;
    }
}