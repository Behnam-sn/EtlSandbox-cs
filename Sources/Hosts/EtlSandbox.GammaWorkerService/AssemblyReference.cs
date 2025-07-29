using System.Reflection;

namespace EtlSandbox.GammaWorkerService;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}