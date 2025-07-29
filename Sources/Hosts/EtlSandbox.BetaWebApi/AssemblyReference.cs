using System.Reflection;

namespace EtlSandbox.BetaWebApi;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}