using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using DanmaobCfdi.Catalogs;
using DanmaobCfdi.Catalogs.Cfdi40;
using DanmaobCfdi.Catalogs.Persistence;
using DanmaobCfdi.Catalogs.Query;
using DanmaobCfdi.Core.Catalogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DanmaobCfdi.Tests.Catalogs.Query;

public sealed class SatCatalogQueryTests
{
    [Fact]
    public async Task Find_ExistingKey_ReturnsEntry()
    {
        using SatDbContext context = CreateContext();
        context.Set<Aduana>().Add(new Aduana { Clave = "01", Descripcion = "ACAPULCO, GUERRERO" });
        context.SaveChanges();
        SatCatalogQuery query = new SatCatalogQuery(context);
        SatCatalogEntry? entry = await query.FindAsync(SatCatalog.Aduana, "01", null, CancellationToken.None);
        Assert.NotNull(entry);
        Assert.Equal("ACAPULCO, GUERRERO", entry.Descripcion);
        Assert.Null(entry.ClavePadre);
    }

    [Fact]
    public async Task Find_MissingKey_ReturnsNull()
    {
        using SatDbContext context = CreateContext();
        SatCatalogQuery query = new SatCatalogQuery(context);
        SatCatalogEntry? entry = await query.FindAsync(SatCatalog.Aduana, "99", null, CancellationToken.None);
        Assert.Null(entry);
    }

    [Fact]
    public async Task Find_CompositeKey_ReturnsTheEntryOfThatParent()
    {
        using SatDbContext context = CreateContext();
        context.Set<Colonia>().Add(new Colonia { Clave = "0001", CodigoPostal = "06300", Descripcion = "Centro" });
        context.Set<Colonia>().Add(new Colonia { Clave = "0001", CodigoPostal = "64000", Descripcion = "Monterrey Centro" });
        context.SaveChanges();
        SatCatalogQuery query = new SatCatalogQuery(context);
        SatCatalogEntry? entry = await query.FindAsync(SatCatalog.Colonia, "0001", "64000", CancellationToken.None);
        Assert.NotNull(entry);
        Assert.Equal("Monterrey Centro", entry.Descripcion);
        Assert.Equal("64000", entry.ClavePadre);
    }

    [Fact]
    public async Task Find_CompositeKeyWithoutParent_Throws()
    {
        using SatDbContext context = CreateContext();
        SatCatalogQuery query = new SatCatalogQuery(context);
        await Assert.ThrowsAsync<ArgumentException>(() => query.FindAsync(SatCatalog.Colonia, "0001", null, CancellationToken.None));
    }

    [Fact]
    public async Task Search_ManyMatches_NeverReturnsMoreThanTheLimit()
    {
        using SatDbContext context = CreateContext();
        for (int index = 0; index < 30; index++)
        {
            context.Set<ClaveProdServ>().Add(new ClaveProdServ { Clave = "101015" + index.ToString("D2", CultureInfo.InvariantCulture), Descripcion = "Producto de prueba" });
        }
        context.SaveChanges();
        SatCatalogQuery query = new SatCatalogQuery(context);
        IReadOnlyList<SatCatalogEntry> results = await query.SearchAsync(SatCatalog.ClaveProdServ, "Producto", null, SatCatalogRules.DefaultSearchLimit, CancellationToken.None);
        Assert.Equal(SatCatalogRules.DefaultSearchLimit, results.Count);
    }

    [Fact]
    public async Task Search_KeyPrefix_ReturnsMatchesOrderedByKey()
    {
        using SatDbContext context = CreateContext();
        context.Set<Aduana>().Add(new Aduana { Clave = "2010", Descripcion = "Segunda" });
        context.Set<Aduana>().Add(new Aduana { Clave = "1011", Descripcion = "Tercera" });
        context.Set<Aduana>().Add(new Aduana { Clave = "1010", Descripcion = "Primera" });
        context.SaveChanges();
        SatCatalogQuery query = new SatCatalogQuery(context);
        IReadOnlyList<SatCatalogEntry> results = await query.SearchAsync(SatCatalog.Aduana, "101", null, SatCatalogRules.DefaultSearchLimit, CancellationToken.None);
        Assert.Equal(2, results.Count);
        Assert.Equal("1010", results[0].Clave);
        Assert.Equal("1011", results[1].Clave);
    }

    [Fact]
    public async Task Search_WithParent_ReturnsOnlyItsChildren()
    {
        using SatDbContext context = CreateContext();
        context.Set<Colonia>().Add(new Colonia { Clave = "0001", CodigoPostal = "06300", Descripcion = "Colonia Centro" });
        context.Set<Colonia>().Add(new Colonia { Clave = "0002", CodigoPostal = "06300", Descripcion = "Colonia Guerrero" });
        context.Set<Colonia>().Add(new Colonia { Clave = "0001", CodigoPostal = "64000", Descripcion = "Colonia Monterrey Centro" });
        context.SaveChanges();
        SatCatalogQuery query = new SatCatalogQuery(context);
        IReadOnlyList<SatCatalogEntry> results = await query.SearchAsync(SatCatalog.Colonia, "Colonia", "06300", SatCatalogRules.DefaultSearchLimit, CancellationToken.None);
        Assert.Equal(2, results.Count);
        Assert.All(results, entry => Assert.Equal("06300", entry.ClavePadre));
    }

    [Fact]
    public async Task List_SmallCatalog_ReturnsAllOrderedByKey()
    {
        using SatDbContext context = CreateContext();
        context.Set<FormaPago>().Add(new FormaPago { Clave = "03", Descripcion = "Transferencia" });
        context.Set<FormaPago>().Add(new FormaPago { Clave = "01", Descripcion = "Efectivo" });
        context.SaveChanges();
        SatCatalogQuery query = new SatCatalogQuery(context);
        IReadOnlyList<SatCatalogEntry> results = await query.ListAsync(SatCatalog.FormaPago, CancellationToken.None);
        Assert.Equal(2, results.Count);
        Assert.Equal("01", results[0].Clave);
    }

    [Fact]
    public async Task List_LargeCatalog_Throws()
    {
        using SatDbContext context = CreateContext();
        SatCatalogQuery query = new SatCatalogQuery(context);
        await Assert.ThrowsAsync<InvalidOperationException>(() => query.ListAsync(SatCatalog.ClaveProdServ, CancellationToken.None));
    }

    [Fact]
    public async Task FakeList_LargeCatalog_Throws()
    {
        FakeSatCatalogQuery query = new();
        await Assert.ThrowsAsync<InvalidOperationException>(() => query.ListAsync(SatCatalog.Colonia, CancellationToken.None));
    }

    [Fact]
    public void IsSmall_LargeAndSmallCatalogs_FollowTheCounts()
    {
        Assert.False(SatCatalogRules.IsSmall(SatCatalog.ClaveProdServ));
        Assert.False(SatCatalogRules.IsSmall(SatCatalog.Colonia));
        Assert.False(SatCatalogRules.IsSmall(SatCatalog.FraccionArancelaria));
        Assert.True(SatCatalogRules.IsSmall(SatCatalog.FormaPago));
        Assert.True(SatCatalogRules.IsSmall(SatCatalog.Localidad));
    }

    [Fact]
    public void AddDanmaobCfdiCatalogs_ConnectionString_RegistersScopedCatalogQuery()
    {
        ServiceCollection services = new ServiceCollection();
        services.AddDanmaobCfdiCatalogs("Server=localhost;Database=DanmaobCfdiRegistrationTests;TrustServerCertificate=True");
        ServiceDescriptor descriptor = Assert.Single(services, service => service.ServiceType == typeof(ISatCatalogQuery));
        Assert.Equal(typeof(SatCatalogQuery), descriptor.ImplementationType);
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    private static SatDbContext CreateContext()
    {
        DbContextOptions<SatDbContext> options = new DbContextOptionsBuilder<SatDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        return new SatDbContext(options);
    }
}
