namespace EtlSandbox.Domain.Common.Options.WorkerSettings;

public sealed class InsertWorkerSettings<T> : BaseWorkerSettings<T>
{
    public long StartingPointId { get; set; }
}