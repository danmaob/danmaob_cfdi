using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using DanmaobCfdi.Catalogs.Cfdi40;
using DanmaobCfdi.Catalogs.Persistence;
using DanmaobCfdi.Catalogs.Query;
using DanmaobCfdi.Core.Catalogs;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DanmaobCfdi.Tests.Catalogs.Query;

public sealed class SatCatalogValidityTests
{
    private const string DateFormat = "yyyy-MM-dd";
    private static readonly DateOnly ComprobanteDate = new DateOnly(2026, 11, 4);

    [Theory]
    [InlineData(null, null, "2026-11-04", true)]
    [InlineData("2026-11-04", null, "2026-11-04", true)]
    [InlineData("2026-11-05", null, "2026-11-04", false)]
    [InlineData(null, "2026-11-04", "2026-11-04", true)]
    [InlineData(null, "2026-11-03", "2026-11-04", false)]
    [InlineData("2022-01-01", "2026-12-31", "2026-11-04", true)]
    public void IsValidOn_Dates_FollowTheValidityRule(string? start, string? end, string date, bool expected)
    {
        SatCatalogEntry entry = new SatCatalogEntry("01", null, "Prueba", ParseOptional(start), ParseOptional(end));
        bool valid = SatCatalogRules.IsValidOn(entry, DateOnly.ParseExact(date, DateFormat, CultureInfo.InvariantCulture));
        Assert.Equal(expected, valid);
    }

    [Fact]
    public async Task CheckValidity_ValidKey_ReturnsValid()
    {
        using SatDbContext context = CreateContext();
        context.Set<Aduana>().Add(new Aduana { Clave = "01", Descripcion = "ACAPULCO, GUERRERO", FechaInicioVigencia = new DateOnly(2022, 1, 1) });
        context.SaveChanges();
        SatCatalogQuery query = new SatCatalogQuery(context);
        SatCatalogValidity validity = await query.CheckValidityAsync(SatCatalog.Aduana, "01", null, ComprobanteDate, CancellationToken.None);
        Assert.Equal(SatCatalogValidity.Valid, validity);
    }

    [Fact]
    public async Task CheckValidity_ExpiredKey_ReturnsNotValid()
    {
        using SatDbContext context = CreateContext();
        context.Set<Aduana>().Add(new Aduana { Clave = "01", Descripcion = "ACAPULCO, GUERRERO", FechaFinVigencia = new DateOnly(2025, 12, 31) });
        context.SaveChanges();
        SatCatalogQuery query = new SatCatalogQuery(context);
        SatCatalogValidity validity = await query.CheckValidityAsync(SatCatalog.Aduana, "01", null, ComprobanteDate, CancellationToken.None);
        Assert.Equal(SatCatalogValidity.NotValid, validity);
    }

    [Fact]
    public async Task CheckValidity_MissingKey_ReturnsNotFound()
    {
        using SatDbContext context = CreateContext();
        SatCatalogQuery query = new SatCatalogQuery(context);
        SatCatalogValidity validity = await query.CheckValidityAsync(SatCatalog.Aduana, "99", null, ComprobanteDate, CancellationToken.None);
        Assert.Equal(SatCatalogValidity.NotFound, validity);
    }

    [Fact]
    public async Task CheckValidity_CompositeKeyWithoutParent_Throws()
    {
        using SatDbContext context = CreateContext();
        SatCatalogQuery query = new SatCatalogQuery(context);
        await Assert.ThrowsAsync<ArgumentException>(() => query.CheckValidityAsync(SatCatalog.Colonia, "0001", null, ComprobanteDate, CancellationToken.None));
    }

    [Fact]
    public async Task FakeCheckValidity_ExpiredKey_ReturnsNotValid()
    {
        FakeSatCatalogQuery query = new FakeSatCatalogQuery();
        query.Add(SatCatalog.Aduana, new SatCatalogEntry("01", null, "ACAPULCO, GUERRERO", null, new DateOnly(2025, 12, 31)));
        SatCatalogValidity validity = await query.CheckValidityAsync(SatCatalog.Aduana, "01", null, ComprobanteDate, CancellationToken.None);
        Assert.Equal(SatCatalogValidity.NotValid, validity);
    }

    [Fact]
    public async Task FakeCheckValidity_MissingKey_ReturnsNotFound()
    {
        FakeSatCatalogQuery query = new FakeSatCatalogQuery();
        SatCatalogValidity validity = await query.CheckValidityAsync(SatCatalog.Aduana, "99", null, ComprobanteDate, CancellationToken.None);
        Assert.Equal(SatCatalogValidity.NotFound, validity);
    }

    private static DateOnly? ParseOptional(string? text)
    {
        if (text is null)
        {
            return null;
        }
        return DateOnly.ParseExact(text, DateFormat, CultureInfo.InvariantCulture);
    }

    private static SatDbContext CreateContext()
    {
        DbContextOptions<SatDbContext> options = new DbContextOptionsBuilder<SatDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        return new SatDbContext(options);
    }
}
