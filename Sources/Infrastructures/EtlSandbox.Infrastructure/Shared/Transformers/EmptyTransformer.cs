using EtlSandbox.Domain.Shared;

namespace EtlSandbox.Infrastructure.Shared.Transformers;

public sealed class EmptyTransformer<T> : ITransformer<T>
    where T : class, IEntity
{
    public T Transform(T input)
    {
        return input;
    }
}