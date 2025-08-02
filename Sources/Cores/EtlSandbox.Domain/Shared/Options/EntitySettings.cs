namespace EtlSandbox.Domain.Shared.Options;

public sealed class EntitySettings<T>
    where T : class, IEntity
{
    public long StartingPointId { get; set; } = 0;
}