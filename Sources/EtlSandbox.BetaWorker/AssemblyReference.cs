using System.Reflection;

namespace EtlSandbox.BetaWorker;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}