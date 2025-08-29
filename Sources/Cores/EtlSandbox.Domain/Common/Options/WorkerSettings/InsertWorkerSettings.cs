namespace EtlSandbox.Domain.Common.Options.WorkerSettings;

public sealed class InsertWorkerSettings<T> : WorkerSettings<T>
{
    public long? StartingPointId { get; set; }
}