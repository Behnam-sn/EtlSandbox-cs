namespace EtlSandbox.Domain.ApplicationStates;

public class ApplicationState
{
    public int Id { get; set; }

    public string EntityType { get; set; }

    public ActionType ActionType { get; set; }

    public int LastProcessedId { get; set; }
}