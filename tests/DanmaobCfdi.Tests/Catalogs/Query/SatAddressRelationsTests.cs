using System;
using System.Collections.Generic;
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

public sealed class SatAddressRelationsTests
{
    [Fact]
    public async Task Check_CongruentAddress_ReturnsNoIssues()
    {
        IReadOnlyList<SatAddressIssue> issues = await CheckAsync(new SatAddress("MEX", "JAL", "44100", "039", "02", "0001"));
        Assert.Empty(issues);
    }

    [Fact]
    public async Task Check_ColoniaOfAnotherCodigoPostal_ReportsColoniaNotFound()
    {
        IReadOnlyList<SatAddressIssue> issues = await CheckAsync(new SatAddress("MEX", "JAL", "44100", "039", "02", "0002"));
        Assert.Equal(new SatAddressIssue[] { SatAddressIssue.ColoniaNotFound }, issues);
    }

    [Fact]
    public async Task Check_EstadoOfAnotherPais_ReportsEstadoNotFoundAndMismatch()
    {
        IReadOnlyList<SatAddressIssue> issues = await CheckAsync(new SatAddress("MEX", "TX", "44100", null, null, null));
        Assert.Equal(new SatAddressIssue[] { SatAddressIssue.EstadoNotFound, SatAddressIssue.CodigoPostalEstadoMismatch }, issues);
    }

    [Fact]
    public async Task Check_MissingCodigoPostal_ReportsCodigoPostalNotFound()
    {
        IReadOnlyList<SatAddressIssue> issues = await CheckAsync(new SatAddress("MEX", "JAL", "99999", null, null, null));
        Assert.Equal(new SatAddressIssue[] { SatAddressIssue.CodigoPostalNotFound }, issues);
    }

    [Fact]
    public async Task Check_MunicipioOfAnotherEstado_ReportsMismatchAndMunicipioNotFound()
    {
        IReadOnlyList<SatAddressIssue> issues = await CheckAsync(new SatAddress("MEX", "JAL", "44100", "120", null, null));
        Assert.Equal(new SatAddressIssue[] { SatAddressIssue.CodigoPostalMunicipioMismatch, SatAddressIssue.MunicipioNotFound }, issues);
    }

    [Fact]
    public async Task Check_LocalidadWithEmptyCodigoPostalColumn_ReportsLocalidadMismatch()
    {
        IReadOnlyList<SatAddressIssue> issues = await CheckAsync(new SatAddress("MEX", "JAL", "44200", "039", "02", null));
        Assert.Equal(new SatAddressIssue[] { SatAddressIssue.CodigoPostalLocalidadMismatch }, issues);
    }

    [Fact]
    public async Task Check_UnitedStatesAddress_ChecksOnlyTheEstado()
    {
        IReadOnlyList<SatAddressIssue> issues = await CheckAsync(new SatAddress("USA", "TX", "75001", "Dallas", "Addison", "Centro"));
        Assert.Empty(issues);
    }

    [Fact]
    public async Task Check_OtherPais_ReturnsNoIssues()
    {
        IReadOnlyList<SatAddressIssue> issues = await CheckAsync(new SatAddress("DEU", "BE", "10115", "Berlin", "Mitte", "Zentrum"));
        Assert.Empty(issues);
    }

    [Fact]
    public async Task FindCodigoPostal_ExistingKey_ReturnsItsColumns()
    {
        using SatDbContext context = CreateContext();
        context.Set<CodigoPostal>().Add(new CodigoPostal { Clave = "44100", Estado = "JAL", Municipio = "039", Localidad = "02" });
        context.SaveChanges();
        SatCatalogQuery query = new SatCatalogQuery(context);
        SatCodigoPostalEntry? entry = await query.FindCodigoPostalAsync("44100", CancellationToken.None);
        Assert.NotNull(entry);
        Assert.Equal("JAL", entry.Estado);
        Assert.Equal("039", entry.Municipio);
        Assert.Equal("02", entry.Localidad);
    }

    [Fact]
    public async Task FindCodigoPostal_MissingKey_ReturnsNull()
    {
        using SatDbContext context = CreateContext();
        SatCatalogQuery query = new SatCatalogQuery(context);
        SatCodigoPostalEntry? entry = await query.FindCodigoPostalAsync("99999", CancellationToken.None);
        Assert.Null(entry);
    }

    [Fact]
    public void AddDanmaobCfdiCatalogs_ConnectionString_RegistersScopedAddressRelations()
    {
        ServiceCollection services = new ServiceCollection();
        services.AddDanmaobCfdiCatalogs("Server=localhost;Database=DanmaobCfdiRegistrationTests;TrustServerCertificate=True");
        ServiceDescriptor descriptor = Assert.Single(services, service => service.ServiceType == typeof(SatAddressRelations));
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    private static async Task<IReadOnlyList<SatAddressIssue>> CheckAsync(SatAddress address)
    {
        SatAddressRelations relations = new SatAddressRelations(CreateQuery());
        IReadOnlyList<SatAddressIssue> issues = await relations.CheckAsync(address, CancellationToken.None);
        return issues;
    }

    private static FakeSatCatalogQuery CreateQuery()
    {
        FakeSatCatalogQuery query = new FakeSatCatalogQuery();
        query.Add(SatCatalog.Estado, new SatCatalogEntry("JAL", "MEX", "Jalisco", null, null));
        query.Add(SatCatalog.Estado, new SatCatalogEntry("TX", "USA", "Texas", null, null));
        query.Add(SatCatalog.Municipio, new SatCatalogEntry("039", "JAL", "Guadalajara", null, null));
        query.Add(SatCatalog.Localidad, new SatCatalogEntry("02", "JAL", "Guadalajara", null, null));
        query.Add(SatCatalog.Colonia, new SatCatalogEntry("0001", "44100", "Centro", null, null));
        query.Add(SatCatalog.Colonia, new SatCatalogEntry("0002", "44200", "Americana", null, null));
        query.AddCodigoPostal(new SatCodigoPostalEntry("44100", "JAL", "039", "02"));
        query.AddCodigoPostal(new SatCodigoPostalEntry("44200", "JAL", "039", null));
        return query;
    }

    private static SatDbContext CreateContext()
    {
        DbContextOptions<SatDbContext> options = new DbContextOptionsBuilder<SatDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        return new SatDbContext(options);
    }
}
