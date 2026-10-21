using System;
using System.Reflection;
using DanmaobCfdi.Core;
using Xunit;

namespace DanmaobCfdi.Tests.Core;

public sealed class ComponentVersionTests
{
    private static string ReadExpectedVersion()
    {
        foreach (AssemblyMetadataAttribute attribute in typeof(ComponentVersionTests).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>())
        {
            if (attribute.Key == "ExpectedComponentVersion" && attribute.Value is not null)
            {
                return attribute.Value;
            }
        }
        throw new InvalidOperationException("The test assembly has no metadata ExpectedComponentVersion.");
    }

    [Fact]
    public void Current_ReadFromAssembly_EqualsBuildVersion()
    {
        string expected = ReadExpectedVersion();
        Assert.Equal(expected, ComponentVersion.Current);
        Assert.DoesNotContain("+", ComponentVersion.Current);
    }

    [Fact]
    public void Current_ReadFromAssembly_MatchesAssemblyAndFileVersion()
    {
        Assembly assembly = typeof(ComponentVersion).Assembly;
        string numericVersion = ComponentVersion.Current.Split('-')[0] + ".0";
        Assert.Equal(numericVersion, assembly.GetName().Version?.ToString());
        Assert.Equal(numericVersion, assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version);
    }
}
