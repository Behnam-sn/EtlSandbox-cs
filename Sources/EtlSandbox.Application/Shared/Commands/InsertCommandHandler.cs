using EtlSandbox.Application.Shared.Abstractions.Messaging;
using EtlSandbox.Domain.ApplicationStates.Enums;
using EtlSandbox.Domain.ApplicationStates.Repositories;
using EtlSandbox.Domain.Shared;

namespace EtlSandbox.Application.Shared.Commands;

public sealed class InsertCommandHandler<T> : ICommandHandler<InsertCommand<T>>
    where T : IEntity
{
    private readonly IApplicationStateCommandRepository _applicationStateCommandRepository;
    private readonly IExtractor<T> _extractor;
    private readonly ITransformer<T> _transformer;
    private readonly ILoader<T> _loader;
    private readonly IUnitOfWork _unitOfWork;

    public InsertCommandHandler(
        IApplicationStateCommandRepository applicationStateCommandRepository,
        IExtractor<T> extractor,
        ITransformer<T> transformer,
        ILoader<T> loader,
        IUnitOfWork unitOfWork
    )
    {
        _applicationStateCommandRepository = applicationStateCommandRepository;
        _extractor = extractor;
        _transformer = transformer;
        _loader = loader;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(InsertCommand<T> request, CancellationToken cancellationToken)
    {
        var lastProcessedId = await _applicationStateCommandRepository.GetLastProcessedIdAsync<T>(ProcessType.Insert);

        var data = await _extractor.ExtractAsync(
            lastProcessedId,
            request.BatchSize,
            cancellationToken
        );

        if (data.Count != 0)
        {
            var transformed = data.Select(_transformer.Transform).ToList();

            _unitOfWork.Connection.Open();
            _unitOfWork.BeginTransaction();

            try
            {
                await _loader.LoadAsync(transformed, cancellationToken, _unitOfWork.Transaction);
                await _applicationStateCommandRepository.UpdateLastProcessedIdAsync<T>(
                    processType: ProcessType.Insert,
                    lastProcessedId: transformed.Max(item => item.Id),
                    transaction: _unitOfWork.Transaction
                );
                _unitOfWork.Commit();
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
            finally
            {
                _unitOfWork.Connection.Close();
            }
        }
    }
}
