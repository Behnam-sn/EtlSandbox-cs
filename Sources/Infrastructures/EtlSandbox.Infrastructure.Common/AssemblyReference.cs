using System.Reflection;

namespace EtlSandbox.Infrastructure.Common;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}