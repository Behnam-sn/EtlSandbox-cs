using EtlSandbox.Domain.Common;
using EtlSandbox.Domain.Common.Resolvers;

namespace EtlSandbox.Infrastructure.Common.Resolvers;

public sealed class SoftDeleteStartingPointResolver<T> : ISoftDeleteStartingPointResolver<T>
    where T : class, IEntity
{
    public long StartingPoint { get; set; } = 0;
}