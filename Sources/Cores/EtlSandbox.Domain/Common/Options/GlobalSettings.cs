namespace EtlSandbox.Domain.Common.Options;

public sealed class GlobalSettings
{
    public int BatchSize { get; set; }
    
    public int MinBatchSize { get; set; }
    
    public int MaxBatchSize { get; set; }

    public int DelayInMilliSeconds { get; set; }
}