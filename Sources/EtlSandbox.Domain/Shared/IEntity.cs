namespace EtlSandbox.Domain.Shared;

public interface IEntity
{
    long Id { get; set; }

    int ImportantId { get; }

    bool IsDeleted { get; set; }
}