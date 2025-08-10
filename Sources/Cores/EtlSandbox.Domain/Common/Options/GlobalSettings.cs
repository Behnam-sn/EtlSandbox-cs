namespace EtlSandbox.Domain.Common.Options;

public sealed class GlobalSettings
{
    public int BatchSize { get; set; }

    public int DelayInMilliSeconds { get; set; }
}