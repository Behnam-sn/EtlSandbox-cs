namespace EtlSandbox.Domain.Shared.Options;

public sealed class DatabaseConnections
{
    public string Source { get; set; } = string.Empty;

    public string Destination { get; set; } = string.Empty;
}
