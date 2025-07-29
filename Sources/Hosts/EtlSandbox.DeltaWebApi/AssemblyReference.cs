using System.Reflection;

namespace EtlSandbox.DeltaWebApi;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}