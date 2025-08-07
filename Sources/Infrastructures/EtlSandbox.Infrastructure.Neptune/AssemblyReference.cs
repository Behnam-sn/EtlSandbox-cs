using System.Reflection;

namespace EtlSandbox.Infrastructure.Neptune;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}