using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using DanmaobCfdi.Core.Catalogs;
using Xunit;

namespace DanmaobCfdi.Tests.Catalogs.Query;

public sealed class SatCatalogQueryContractTests
{
    [Fact]
    public void ValidateSearch_ShortText_Throws()
    {
        Assert.Throws<ArgumentException>(() => SatCatalogRules.ValidateSearch(" ab ", SatCatalogRules.DefaultSearchLimit));
    }

    [Fact]
    public void ValidateSearch_LimitAboveMaximum_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => SatCatalogRules.ValidateSearch("abc", SatCatalogRules.MaxSearchLimit + 1));
    }

    [Fact]
    public void ValidateSearch_LimitZero_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => SatCatalogRules.ValidateSearch("abc", 0));
    }

    [Fact]
    public void ValidateSearch_ValidTextAndLimit_DoesNotThrow()
    {
        Exception? exception = Record.Exception(() => SatCatalogRules.ValidateSearch("abc", SatCatalogRules.MaxSearchLimit));
        Assert.Null(exception);
    }

    [Fact]
    public void HasParentKey_CompositeKeyCatalogs_ReturnsTrue()
    {
        Assert.True(SatCatalogRules.HasParentKey(SatCatalog.Colonia));
        Assert.True(SatCatalogRules.HasParentKey(SatCatalog.Estado));
        Assert.True(SatCatalogRules.HasParentKey(SatCatalog.Municipio));
        Assert.True(SatCatalogRules.HasParentKey(SatCatalog.Localidad));
        Assert.False(SatCatalogRules.HasParentKey(SatCatalog.Pais));
    }

    [Fact]
    public async Task Search_ManyMatches_NeverReturnsMoreThanTheLimit()
    {
        FakeSatCatalogQuery query = new FakeSatCatalogQuery();
        for (int index = 0; index < 30; index++)
        {
            query.Add(SatCatalog.ClaveProdServ, new SatCatalogEntry("101" + index.ToString("D2", CultureInfo.InvariantCulture), null, "Producto de prueba", null, null));
        }

        IReadOnlyList<SatCatalogEntry> results = await query.SearchAsync(SatCatalog.ClaveProdServ, "producto", null, SatCatalogRules.DefaultSearchLimit, CancellationToken.None);
        Assert.Equal(SatCatalogRules.DefaultSearchLimit, results.Count);
    }

    [Fact]
    public async Task Search_WithParentKey_ReturnsOnlyItsChildren()
    {
        FakeSatCatalogQuery query = new FakeSatCatalogQuery();
        query.Add(SatCatalog.Colonia, new SatCatalogEntry("0001", "06300", "Colonia Centro", null, null));
        query.Add(SatCatalog.Colonia, new SatCatalogEntry("0002", "06300", "Colonia Guerrero", null, null));
        query.Add(SatCatalog.Colonia, new SatCatalogEntry("0001", "64000", "Colonia Monterrey Centro", null, null));

        IReadOnlyList<SatCatalogEntry> results = await query.SearchAsync(SatCatalog.Colonia, "colonia", "06300", SatCatalogRules.DefaultSearchLimit, CancellationToken.None);
        Assert.Equal(2, results.Count);
        Assert.All(results, entry => Assert.Equal("06300", entry.ClavePadre));
    }

    [Fact]
    public async Task Find_CompositeKey_ReturnsTheEntryOfThatParent()
    {
        FakeSatCatalogQuery query = new FakeSatCatalogQuery();
        query.Add(SatCatalog.Colonia, new SatCatalogEntry("0001", "06300", "Colonia Centro", null, null));
        query.Add(SatCatalog.Colonia, new SatCatalogEntry("0001", "64000", "Colonia Monterrey Centro", null, null));

        SatCatalogEntry? entry = await query.FindAsync(SatCatalog.Colonia, "0001", "64000", CancellationToken.None);
        Assert.NotNull(entry);
        Assert.Equal("Colonia Monterrey Centro", entry.Descripcion);
    }
}
