using System.Reflection;

namespace EtlSandbox.AlphaWorkerService;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}