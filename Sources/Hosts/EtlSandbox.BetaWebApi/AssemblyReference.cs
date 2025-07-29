using System.Reflection;

namespace EtlSandbox.BetaWebApiService;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}