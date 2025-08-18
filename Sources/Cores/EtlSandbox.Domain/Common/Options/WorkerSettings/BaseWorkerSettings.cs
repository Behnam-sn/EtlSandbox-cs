namespace EtlSandbox.Domain.Common.Options.WorkerSettings;

public abstract class BaseWorkerSettings<T>
{
    public bool Enable { get; set; }

    public int? BatchSize { get; set; }
    
    public int? MinBatchSize { get; set; }
    
    public int? MaxBatchSize { get; set; }

    public int? DelayInMilliSeconds { get; set; }
}