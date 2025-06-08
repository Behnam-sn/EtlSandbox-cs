namespace EtlSandbox.Domain;

public interface ICommandRepository<T>
{
    Task<int> GetLastIdAsync();
}