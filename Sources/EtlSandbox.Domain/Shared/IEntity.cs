namespace EtlSandbox.Domain.Shared;

public interface IEntity
{
    int Id { get; set; }

    int UniqId { get; set; }

    bool IsDeleted { get; set; }
}