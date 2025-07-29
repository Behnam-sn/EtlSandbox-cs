using System.Reflection;

namespace EtlSandbox.GammaWorker;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}