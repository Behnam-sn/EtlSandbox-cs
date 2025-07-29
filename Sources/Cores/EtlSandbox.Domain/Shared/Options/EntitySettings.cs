namespace EtlSandbox.Domain.Shared.Options;

public sealed class EntitySettings<T>
    where T : class, IEntity
{
    public long LastInsertedId { get; set; } = 0;
}