using System.Data;

using EtlSandbox.Domain.EtlApplicationStates.Enums;

namespace EtlSandbox.Domain.EtlApplicationStates.Repositories;

public interface IEtlApplicationStateCommandRepository
{
    Task<int> GetLastProcessedIdAsync<T>(ProcessType processType);

    Task UpdateLastProcessedIdAsync<T>(ProcessType processType, int lastProcessedId, IDbTransaction? transaction = null);
}