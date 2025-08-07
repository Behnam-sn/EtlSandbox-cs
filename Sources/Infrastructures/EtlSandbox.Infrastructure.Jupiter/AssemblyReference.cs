using System.Reflection;

namespace EtlSandbox.Infrastructure.Jupiter;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}