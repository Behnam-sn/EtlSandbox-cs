namespace EtlSandbox.Domain.ApplicationStates;

public interface IApplicationStateCommandRepository
{
    Task<int> GetLastProcessedIdAsync<T>(ActionType actionType);

    Task UpdateLastProcessedIdAsync<T>(ActionType actionType, int lastProcessedId);
}