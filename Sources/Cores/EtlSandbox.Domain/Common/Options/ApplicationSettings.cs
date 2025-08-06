namespace EtlSandbox.Domain.Common.Options;

public sealed class ApplicationSettings
{
    public int BatchSize { get; set; } = 1000;

    public int DelayInSeconds { get; set; } = 10;
}