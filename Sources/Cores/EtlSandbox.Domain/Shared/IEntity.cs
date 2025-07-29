namespace EtlSandbox.Domain.Shared;

public interface IEntity
{
    long Id { get; set; }

    long ImportantId { get; }

    bool IsDeleted { get; set; }
}