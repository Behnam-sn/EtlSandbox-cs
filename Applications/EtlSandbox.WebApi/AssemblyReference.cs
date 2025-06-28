using System.Reflection;

namespace EtlSandbox.WebApi;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}