namespace EtlSandbox.Domain.Shared;

public interface ICommandRepository<T>
{
    Task<int> GetLastIdAsync();
}