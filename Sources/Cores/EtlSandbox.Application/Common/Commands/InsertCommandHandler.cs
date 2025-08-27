using EtlSandbox.Application.Common.Abstractions.Messaging;
using EtlSandbox.Domain.Common;
using EtlSandbox.Domain.Common.Repositories;
using EtlSandbox.Domain.Common.Resolvers;

using Microsoft.Extensions.Logging;

namespace EtlSandbox.Application.Common.Commands;

public sealed class InsertCommandHandler<TSource, TDestination> : ICommandHandler<InsertCommand<TSource, TDestination>>
    where TSource : class
    where TDestination : class, IEntity
{
    private readonly ILogger _logger;

    // Todo: Rename _insertStartingPointResolver to _startingPointResolver
    private readonly IInsertStartingPointResolver<TSource, TDestination> _insertStartingPointResolver;

    private readonly ISourceRepository<TSource> _sourceRepository;

    private readonly IExtractor<TDestination> _extractor;

    private readonly ITransformer<TDestination> _transformer;

    private readonly ILoader<TDestination> _loader;

    public InsertCommandHandler(
        ILogger<InsertCommandHandler<TSource, TDestination>> logger,
        IInsertStartingPointResolver<TSource, TDestination> insertStartingPointResolver,
        IExtractor<TDestination> extractor,
        ITransformer<TDestination> transformer,
        ILoader<TDestination> loader, ISourceRepository<TSource> sourceRepository)
    {
        _logger = logger;
        _insertStartingPointResolver = insertStartingPointResolver;
        _extractor = extractor;
        _transformer = transformer;
        _loader = loader;
        _sourceRepository = sourceRepository;
    }

    public async Task Handle(InsertCommand<TSource, TDestination> request, CancellationToken cancellationToken)
    {
        var sourceLastId = await _sourceRepository.GetMaxIdOrDefaultAsync(cancellationToken);
        var from = await _insertStartingPointResolver.GetStartingPointAsync(settingsStartingPoint: request.StartingPointId);
        var to = from + request.BatchSize < sourceLastId
            ? from + request.BatchSize
            : sourceLastId;

        _logger.LogInformation("Extracting data from {From} to {To}", from, to);
        var extractedItems = await _extractor.ExtractAsync(from, to, cancellationToken);
        _logger.LogInformation("Extracted {Count} rows", extractedItems.Count);

        if (extractedItems.Count == 0)
        {
            return;
        }

        _logger.LogInformation("Transforming items");
        var transformedItems = extractedItems.AsParallel().Select(_transformer.Transform).ToList();
        _logger.LogInformation("Transformed {Count} rows", transformedItems.Count);

        _logger.LogInformation("Loading");
        await _loader.LoadAsync(transformedItems, cancellationToken);
        _logger.LogInformation("Loaded {Count} rows", extractedItems.Count);

        _insertStartingPointResolver.SetStartingPoint(to);
    }
}