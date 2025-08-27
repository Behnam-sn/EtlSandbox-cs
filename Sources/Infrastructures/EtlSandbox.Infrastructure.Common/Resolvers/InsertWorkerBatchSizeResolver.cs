using EtlSandbox.Domain.Common;
using EtlSandbox.Domain.Common.Options;
using EtlSandbox.Domain.Common.Options.WorkerSettings;
using EtlSandbox.Domain.Common.Repositories;
using EtlSandbox.Domain.Common.Resolvers;

using Microsoft.Extensions.Options;


namespace EtlSandbox.Infrastructure.Common.Resolvers;

public sealed class InsertWorkerBatchSizeResolver<TWorker ,TSource, TDestination> : IInsertWorkerBatchSizeResolver<TWorker, TSource, TDestination>
    where TWorker : class
    where TSource : class
    where TDestination : class, IEntity
{
    private readonly GlobalSettings  _globalSettings;
    
    private readonly InsertWorkerSettings<TWorker>  _insertWorkerSettings;
    
    private readonly ISourceRepository<TSource> _sourceRepository;
    
    private readonly IDestinationRepository<TDestination> _destinationRepository;

    public InsertWorkerBatchSizeResolver(
        IOptions<GlobalSettings> globalSettingsOptions,
        IOptions<InsertWorkerSettings<TWorker>> insertWorkerSettingsOptions,
        ISourceRepository<TSource> sourceRepository,
        IDestinationRepository<TDestination> destinationRepository
    )
    {
        _globalSettings = globalSettingsOptions.Value;
        _insertWorkerSettings = insertWorkerSettingsOptions.Value;
        _sourceRepository = sourceRepository;
        _destinationRepository = destinationRepository;
    }

    public async Task<int> GetBatchSizeAsync()
    {
        var minBatchSize = _insertWorkerSettings.MinBatchSize ?? _globalSettings.MinBatchSize;
        var maxBatchSize = _insertWorkerSettings.MaxBatchSize  ?? _globalSettings.MaxBatchSize;

        var sourceLastIdTask = _sourceRepository.GetMaxIdOrDefaultAsync();
        var destinationLastSourceIdTask = _destinationRepository.GetMaxSourceIdOrDefaultAsync();

        await Task.WhenAll(sourceLastIdTask, destinationLastSourceIdTask);

        var sourceLastId = sourceLastIdTask.Result;
        var destinationLastSourceId = destinationLastSourceIdTask.Result;
        var gap = sourceLastId -  destinationLastSourceId;

        return gap < minBatchSize ? minBatchSize : maxBatchSize;
    }
}