namespace EtlSandbox.Domain.Shared;

public interface IEtlStateCommandRepository
{
    Task<int> GetLastProcessedIdAsync();

    Task UpdateLastProcessedIdAsync(int lastProcessedId);
}