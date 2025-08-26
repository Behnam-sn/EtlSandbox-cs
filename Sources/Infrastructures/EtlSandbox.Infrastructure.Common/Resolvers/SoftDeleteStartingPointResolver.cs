using EtlSandbox.Domain.Common;
using EtlSandbox.Domain.Common.Repositories;
using EtlSandbox.Domain.Common.Resolvers;

using Microsoft.Extensions.DependencyInjection;

namespace EtlSandbox.Infrastructure.Common.Resolvers;

public sealed class SoftDeleteStartingPointResolver<T> : ISoftDeleteStartingPointResolver<T>
    where T : class, IEntity
{
    public long StartingPoint { get; set; } = 0;
}