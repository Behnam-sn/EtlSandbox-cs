namespace EtlSandbox.Domain.Common.Options;

public sealed class ApplicationSettings
{
    public int BatchSize { get; set; } = 100;

    public int DelayInMilliSeconds { get; set; } = 100;
}