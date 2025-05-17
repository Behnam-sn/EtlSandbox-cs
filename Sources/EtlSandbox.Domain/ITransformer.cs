namespace EtlSandbox.Domain;

public interface ITransformer<T>
{
    T Transform(T input);
}
