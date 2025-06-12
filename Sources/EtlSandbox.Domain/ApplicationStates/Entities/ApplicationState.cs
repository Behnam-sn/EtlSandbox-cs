using EtlSandbox.Domain.ApplicationStates.Enums;

namespace EtlSandbox.Domain.ApplicationStates.Entities;

public class ApplicationState
{
    public int Id { get; set; }

    public string EntityType { get; set; }

    public ProcessType ProcessType { get; set; }

    public int LastProcessedId { get; set; }
}