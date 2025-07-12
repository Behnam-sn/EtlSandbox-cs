using EtlSandbox.Application.Shared.Abstractions.Messaging;
using EtlSandbox.Domain.EtlApplicationStates.Enums;
using EtlSandbox.Domain.EtlApplicationStates.Repositories;
using EtlSandbox.Domain.Shared;

namespace EtlSandbox.Application.Shared.Commands;

public sealed class SoftDeleteCommandHandler<T> : ICommandHandler<SoftDeleteCommand<T>>
    where T : IEntity
{
    private readonly IEtlApplicationStateCommandRepository _etlApplicationStateCommandRepository;

    private readonly IUnitOfWork _unitOfWork;

    private readonly ISynchronizer<T> _synchronizer;

    public SoftDeleteCommandHandler(IEtlApplicationStateCommandRepository etlApplicationStateCommandRepository, IUnitOfWork unitOfWork, ISynchronizer<T> synchronizer)
    {
        _etlApplicationStateCommandRepository = etlApplicationStateCommandRepository;
        _unitOfWork = unitOfWork;
        _synchronizer = synchronizer;
    }

    public async Task Handle(SoftDeleteCommand<T> request, CancellationToken cancellationToken)
    {
        var lastDeletedId = await _etlApplicationStateCommandRepository.GetLastProcessedIdAsync<T>(ProcessType.Delete);
        var lastInsertedId = await _etlApplicationStateCommandRepository.GetLastProcessedIdAsync<T>(ProcessType.Insert);
        var count = lastInsertedId - lastDeletedId;

        if (count > 0)
        {
            _unitOfWork.Connection.Open();
            _unitOfWork.BeginTransaction();

            try
            {
                var toId = count > request.BatchSize ? lastDeletedId + request.BatchSize : lastInsertedId;

                await _synchronizer.SoftDeleteObsoleteRowsAsync(
                    fromId: lastDeletedId,
                    toId: toId,
                    transaction: _unitOfWork.Transaction
                );

                await _etlApplicationStateCommandRepository.UpdateLastProcessedIdAsync<T>(
                    processType: ProcessType.Delete,
                    lastProcessedId: toId,
                    transaction: _unitOfWork.Transaction
                );

                _unitOfWork.Commit();
            }
            catch (Exception e)
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