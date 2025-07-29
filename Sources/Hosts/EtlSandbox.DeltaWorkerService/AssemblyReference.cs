using System.Reflection;

namespace EtlSandbox.DeltaWorkerService;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}