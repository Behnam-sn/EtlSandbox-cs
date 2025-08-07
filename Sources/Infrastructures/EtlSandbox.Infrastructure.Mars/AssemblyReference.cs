using System.Reflection;

namespace EtlSandbox.Infrastructure.Mars;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}