namespace EtlSandbox.Domain.Common.Options;

public sealed class GlobalSettings
{
    public int MinBatchSize { get; set; }

    public int MaxBatchSize { get; set; }

    public int MinDelayInMilliSeconds { get; set; }

    public int MaxDelayInMilliSeconds { get; set; }
}