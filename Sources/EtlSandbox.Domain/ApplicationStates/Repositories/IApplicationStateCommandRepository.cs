using System.Data;

using EtlSandbox.Domain.ApplicationStates.Enums;

namespace EtlSandbox.Domain.ApplicationStates.Repositories;

public interface IApplicationStateCommandRepository
{
    Task<int> GetLastProcessedIdAsync<T>(ActionType actionType);

    Task UpdateLastProcessedIdAsync<T>(ActionType actionType, int lastProcessedId, IDbTransaction? transaction = null);
}