using System.Reflection;

namespace EtlSandbox.Persistence.Mars;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}