using System.Reflection;

namespace EtlSandbox.Worker;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}