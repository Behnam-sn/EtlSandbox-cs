using System.Reflection;

namespace EtlSandbox.BetaWorkerService;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}