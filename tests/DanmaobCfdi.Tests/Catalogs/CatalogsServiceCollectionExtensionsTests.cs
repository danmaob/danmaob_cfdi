using System;
using DanmaobCfdi.Catalogs;
using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DanmaobCfdi.Tests.Catalogs;

public sealed class CatalogsServiceCollectionExtensionsTests
{
    [Fact]
    public void AddDanmaobCfdiCatalogs_BlankConnectionString_ThrowsArgumentException()
    {
        ServiceCollection services = new ServiceCollection();
        Assert.Throws<ArgumentException>(() => services.AddDanmaobCfdiCatalogs(" "));
    }

    [Fact]
    public void AddDanmaobCfdiCatalogs_ConnectionString_RegistersScopedContext()
    {
        ServiceCollection services = new ServiceCollection();
        services.AddDanmaobCfdiCatalogs("Server=localhost;Database=DanmaobCfdiRegistrationTests;TrustServerCertificate=True");
        ServiceDescriptor descriptor = Assert.Single(services, service => service.ServiceType == typeof(SatDbContext));
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
    }
}
