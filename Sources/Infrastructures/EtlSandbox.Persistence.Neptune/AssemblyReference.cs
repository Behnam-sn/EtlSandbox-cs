using System.Reflection;

namespace EtlSandbox.Persistence.Neptune;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}