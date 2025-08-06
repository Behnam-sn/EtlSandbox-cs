using EtlSandbox.Application.Common.Abstractions.Messaging;
using EtlSandbox.Domain.Common;

using Microsoft.Extensions.Logging;

namespace EtlSandbox.Application.Common.Commands;

public sealed class InsertCommandHandler<TSource, TDestination> : ICommandHandler<InsertCommand<TSource, TDestination>>
    where TSource : class
    where TDestination : class, IEntity
{
    private readonly ILogger _logger;

    private readonly IInsertStartingPointResolver<TSource, TDestination> _insertStartingPointResolver;

    private readonly IExtractor<TDestination> _extractor;

    private readonly ITransformer<TDestination> _transformer;

    private readonly ILoader<TDestination> _loader;

    public InsertCommandHandler(
        ILogger<InsertCommandHandler<TSource, TDestination>> logger,
        IInsertStartingPointResolver<TSource,TDestination> insertStartingPointResolver,
        IExtractor<TDestination> extractor,
        ITransformer<TDestination> transformer,
        ILoader<TDestination> loader)
    {
        _logger = logger;
        _insertStartingPointResolver = insertStartingPointResolver;
        _extractor = extractor;
        _transformer = transformer;
        _loader = loader;
    }

    public async Task Handle(InsertCommand<TSource, TDestination> request, CancellationToken cancellationToken)
    {
        var from = await _insertStartingPointResolver.GetStartingPointAsync(request.BatchSize);
        var to = from + request.BatchSize;

        _logger.LogInformation("Extracting data from {From} to {To}", from, to);
        var extractedItems = await _extractor.ExtractAsync(
            from,
            to,
            cancellationToken
        );
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
    }
}