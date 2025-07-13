namespace EtlSandbox.Domain.Shared;

public interface IEntity
{
    int Id { get; set; }

    int ImportantId { get; }

    bool IsDeleted { get; set; }
}