namespace EtlSandbox.Domain;

public interface IEtlStateCommandRepository
{
    Task<int> GetLastProcessedIdAsync();

    Task UpdateLastProcessedIdAsync(int lastProcessedId);
}