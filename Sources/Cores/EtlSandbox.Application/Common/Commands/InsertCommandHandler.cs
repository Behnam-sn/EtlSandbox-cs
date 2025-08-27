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

    private readonly IInsertStartingPointResolver<TSource, TDestination> _startingPointResolver;

    private readonly ISourceRepository<TSource> _sourceRepository;

    private readonly IExtractor<TDestination> _extractor;

    private readonly ITransformer<TDestination> _transformer;

    private readonly ILoader<TDestination> _loader;

    public InsertCommandHandler(
        ILogger<InsertCommandHandler<TSource, TDestination>> logger,
        IInsertStartingPointResolver<TSource, TDestination> startingPointResolver,
        IExtractor<TDestination> extractor,
        ITransformer<TDestination> transformer,
        ILoader<TDestination> loader, ISourceRepository<TSource> sourceRepository)
    {
        _logger = logger;
        _startingPointResolver = startingPointResolver;
        _extractor = extractor;
        _transformer = transformer;
        _loader = loader;
        _sourceRepository = sourceRepository;
    }

    public async Task Handle(InsertCommand<TSource, TDestination> request, CancellationToken cancellationToken)
    {
        var destinationTypeName = typeof(TDestination).Name;
        var sourceLastId = await _sourceRepository.GetMaxIdOrDefaultAsync(cancellationToken);
        var from = await _startingPointResolver.GetStartingPointAsync(settingsStartingPoint: request.StartingPointId);
        var to = from + request.BatchSize < sourceLastId
            ? from + request.BatchSize
            : sourceLastId;

        if (from >= to)
        {
            return;
        }

        _logger.LogInformation("Extracting {Type} from {From} to {To}", destinationTypeName, from, to);
        var extractedItems = await _extractor.ExtractAsync(from, to, cancellationToken);
        _logger.LogInformation("Extracted {Count} {Type}", extractedItems.Count, destinationTypeName);

        if (extractedItems.Count == 0)
        {
            return;
        }

        _logger.LogInformation("Transforming {Type}", destinationTypeName);
        var transformedItems = extractedItems.AsParallel().Select(_transformer.Transform).ToList();
        _logger.LogInformation("Transformed {Count} {Type}", transformedItems.Count, destinationTypeName);

        _logger.LogInformation("Loading {Type}", destinationTypeName);
        await _loader.LoadAsync(transformedItems, cancellationToken);
        _logger.LogInformation("Loaded {Count} {Type}", extractedItems.Count, destinationTypeName);

        _startingPointResolver.SetStartingPoint(to);
    }
}