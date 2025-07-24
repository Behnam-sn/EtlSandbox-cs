using System.Reflection;

namespace EtlSandbox.DeltaWorker;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}