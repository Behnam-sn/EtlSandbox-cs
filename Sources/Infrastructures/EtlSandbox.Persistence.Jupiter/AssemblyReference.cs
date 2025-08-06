using System.Reflection;

namespace EtlSandbox.Persistence.Jupiter;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}