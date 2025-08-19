namespace EtlSandbox.Domain.Common;

public interface IEntity
{
    long Id { get; set; }

    long SourceId { get; }

    bool IsDeleted { get; set; }
}