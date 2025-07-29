namespace EtlSandbox.Domain.Shared;

public interface ITransformer<T>
    where T : class, IEntity
{
    T Transform(T input);
}
