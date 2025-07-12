using EtlSandbox.Domain.EtlApplicationStates.Enums;

namespace EtlSandbox.Domain.EtlApplicationStates.Entities;

public class EtlApplicationState
{
    public int Id { get; set; }

    public string EntityType { get; set; } = string.Empty;

    public ProcessType ProcessType { get; set; }

    public int LastProcessedId { get; set; }
}