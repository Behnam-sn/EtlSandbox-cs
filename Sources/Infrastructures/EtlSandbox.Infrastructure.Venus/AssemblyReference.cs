using System.Reflection;

namespace EtlSandbox.Infrastructure.Venus;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}