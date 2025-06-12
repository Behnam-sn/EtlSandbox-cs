namespace EtlSandbox.Domain.Shared;

public interface IEntity
{
    int Id { get; }

    int UniqId { get; set; }

    bool IsDeleted { get; set; }
}