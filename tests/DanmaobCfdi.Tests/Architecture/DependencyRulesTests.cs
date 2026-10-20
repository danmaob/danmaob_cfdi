using System.Reflection;
using DanmaobCfdi.Catalogs;
using DanmaobCfdi.Core;
using DanmaobCfdi.Pax;
using NetArchTest.Rules;
using Xunit;

namespace DanmaobCfdi.Tests.Architecture;

public sealed class DependencyRulesTests
{
    private static void AssertHasNoDependencies(Assembly assembly, params string[] forbiddenNamespaces)
    {
        var result = Types.InAssembly(assembly).ShouldNot().HaveDependencyOnAny(forbiddenNamespaces).GetResult();
        var failingTypes = result.FailingTypeNames ?? new string[0];
        Assert.True(result.IsSuccessful, $"Forbidden dependencies found in: {string.Join(", ", failingTypes)}");
    }

    [Fact]
    public void Core_ForbiddenDependencies_None()
    {
        AssertHasNoDependencies(typeof(CoreServiceCollectionExtensions).Assembly, "Microsoft.EntityFrameworkCore", "Microsoft.Data.SqlClient", "System.ServiceModel");
    }

    [Fact]
    public void Catalogs_ForbiddenDependencies_None()
    {
        AssertHasNoDependencies(typeof(CatalogsServiceCollectionExtensions).Assembly, "DanmaobCfdi.Pax", "System.ServiceModel");
    }

    [Fact]
    public void Pax_ForbiddenDependencies_None()
    {
        AssertHasNoDependencies(typeof(PaxServiceCollectionExtensions).Assembly, "DanmaobCfdi.Catalogs", "Microsoft.EntityFrameworkCore");
    }

    [Fact]
    public void Rules_ForbiddenDependencyPresent_ReportViolation()
    {
        var result = Types.InAssembly(typeof(DependencyRulesTests).Assembly).ShouldNot().HaveDependencyOnAny("DanmaobCfdi.Catalogs").GetResult();
        Assert.False(result.IsSuccessful);
    }
}
