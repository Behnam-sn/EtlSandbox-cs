using EtlSandbox.Application.Shared.Abstractions.Messaging;
using EtlSandbox.Domain.Shared;

using Microsoft.Extensions.Logging;

namespace EtlSandbox.Application.Shared.Commands;

public sealed class InsertCommandHandler<T> : ICommandHandler<InsertCommand<T>>
    where T : class, IEntity
{
    private readonly ILogger _logger;

    private readonly IInsertStartingPointResolver<T> _insertStartingPointResolver;

    private readonly IExtractor<T> _extractor;

    private readonly ITransformer<T> _transformer;

    private readonly ILoader<T> _loader;

    public InsertCommandHandler(
        ILogger<InsertCommandHandler<T>> logger,
        IInsertStartingPointResolver<T> insertStartingPointResolver,
        IExtractor<T> extractor,
        ITransformer<T> transformer,
        ILoader<T> loader)
    {
        _logger = logger;
        _insertStartingPointResolver = insertStartingPointResolver;
        _extractor = extractor;
        _transformer = transformer;
        _loader = loader;
    }

    public async Task Handle(InsertCommand<T> request, CancellationToken cancellationToken)
    {
        var lastProcessedId = await _insertStartingPointResolver.GetLastProcessedIdAsync();

        _logger.LogInformation("Extracting data since {LastProcessedId}", lastProcessedId);
        var extractedItems = await _extractor.ExtractAsync(
            lastProcessedId,
            request.BatchSize,
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
        _logger.LogInformation("{Count} rows Loaded", extractedItems.Count);
    }
}
