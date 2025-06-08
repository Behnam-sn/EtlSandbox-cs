namespace EtlSandbox.Domain.Shared;

public interface ITransformer<T>
{
    T Transform(T input);
}
