using System.Data;

using EtlSandbox.Domain.ApplicationStates.Enums;

namespace EtlSandbox.Domain.ApplicationStates.Repositories;

public interface IApplicationStateCommandRepository
{
    Task<int> GetLastProcessedIdAsync<T>(ProcessType processType);

    Task UpdateLastProcessedIdAsync<T>(ProcessType processType, int lastProcessedId, IDbTransaction? transaction = null);
}