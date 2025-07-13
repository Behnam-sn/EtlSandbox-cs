using EtlSandbox.Application.Shared.Abstractions.Messaging;
using EtlSandbox.Domain.EtlApplicationStates.Enums;
using EtlSandbox.Domain.EtlApplicationStates.Repositories;
using EtlSandbox.Domain.Shared;

using Microsoft.Extensions.Logging;

namespace EtlSandbox.Application.Shared.Commands;

public sealed class InsertCommandHandler<T> : ICommandHandler<InsertCommand<T>>
    where T : class, IEntity
{
    private readonly ILogger<InsertCommandHandler<T>> _logger;

    private readonly IEtlApplicationStateCommandRepository _etlApplicationStateCommandRepository;

    private readonly IExtractor<T> _extractor;

    private readonly ITransformer<T> _transformer;

    private readonly ILoader<T> _loader;

    private readonly IUnitOfWork _unitOfWork;

    public InsertCommandHandler(
        ILogger<InsertCommandHandler<T>> logger,
        IUnitOfWork unitOfWork,
        IEtlApplicationStateCommandRepository etlApplicationStateCommandRepository,
        IExtractor<T> extractor,
        ITransformer<T> transformer,
        ILoader<T> loader
    )
    {
        _logger = logger;
        _etlApplicationStateCommandRepository = etlApplicationStateCommandRepository;
        _extractor = extractor;
        _transformer = transformer;
        _loader = loader;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(InsertCommand<T> request, CancellationToken cancellationToken)
    {
        var lastProcessedId = await _etlApplicationStateCommandRepository.GetLastProcessedIdAsync<T>(ProcessType.Insert);

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

        _logger.LogInformation("Transforming data");
        var transformedItems = extractedItems.AsParallel().Select(_transformer.Transform).ToList();
        _logger.LogInformation("Transformed {Count} rows", transformedItems.Count);

        _logger.LogInformation("Opening connection");
        _unitOfWork.Connection.Open();
        _logger.LogInformation("Connection opened");

        _logger.LogInformation("Beginning transaction");
        _unitOfWork.BeginTransaction();
        _logger.LogInformation("Transaction began");

        try
        {
            _logger.LogInformation("Loading {Count} rows", extractedItems.Count);
            await _loader.LoadAsync(transformedItems, cancellationToken, _unitOfWork.Transaction);
            _logger.LogInformation("Load completed");

            _logger.LogInformation("Updating EtlApplicationState");
            var newLastProcessedId = transformedItems.Max(item => item.ImportantId);
            await _etlApplicationStateCommandRepository.UpdateLastProcessedIdAsync<T>(
                processType: ProcessType.Insert,
                lastProcessedId: newLastProcessedId,
                transaction: _unitOfWork.Transaction
            );
            _logger.LogInformation("Inserted Until {LastProcessedId}", newLastProcessedId);

            _logger.LogInformation("Commiting transaction");
            _unitOfWork.Commit();
            _logger.LogInformation("Transaction committed");
        }
        catch
        {
            _logger.LogInformation("Roll backing transaction");
            _unitOfWork.Rollback();
            _logger.LogInformation("Transaction rolled back");
            throw;
        }
        finally
        {
            _logger.LogInformation("Closing connection");
            _unitOfWork.Connection.Close();
            _logger.LogInformation("Connection closed");
        }
    }
}