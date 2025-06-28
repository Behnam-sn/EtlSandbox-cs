using System.Reflection;

namespace EtlSandbox.AlphaWorker;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}