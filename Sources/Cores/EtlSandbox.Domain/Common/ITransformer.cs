namespace EtlSandbox.Domain.Common;

public interface ITransformer<T>
    where T : class, IEntity
{
    T Transform(T input);
}
