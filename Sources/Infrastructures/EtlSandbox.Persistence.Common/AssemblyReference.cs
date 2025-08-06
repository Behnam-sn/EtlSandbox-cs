using System.Reflection;

namespace EtlSandbox.Persistence.Common;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}