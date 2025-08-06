using EtlSandbox.Domain.Common;

namespace EtlSandbox.Infrastructure.Common.Transformers;

public sealed class EmptyTransformer<T> : ITransformer<T>
    where T : class, IEntity
{
    public T Transform(T input)
    {
        return input;
    }
}