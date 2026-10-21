using System;
using System.Reflection;

namespace DanmaobCfdi.Core;

public static class ComponentVersion
{
    public static string Current { get; } = ReadInformationalVersion();

    private static string ReadInformationalVersion()
    {
        AssemblyInformationalVersionAttribute? attribute = typeof(ComponentVersion).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        if (attribute is null)
        {
            throw new InvalidOperationException("The assembly DanmaobCfdi.Core has no informational version.");
        }
        return attribute.InformationalVersion;
    }
}
