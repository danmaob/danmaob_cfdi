using System;
using System.Linq;
using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;

namespace DanmaobCfdi.Tests.Catalogs.Cfdi40;

public sealed class SimpleCatalogModelTests
{
    private const string ConnectionString = "Server=localhost;Database=DanmaobCfdiModelTests;TrustServerCertificate=True";

    [Theory]
    [InlineData("c_Aduana", 2, 100, true)]
    [InlineData("c_Exportacion", 2, 100, true)]
    [InlineData("c_Meses", 2, 50, true)]
    [InlineData("c_MetodoPago", 3, 50, true)]
    [InlineData("c_ObjetoImp", 2, 100, true)]
    [InlineData("c_Periodicidad", 2, 50, true)]
    [InlineData("c_TipoRelacion", 2, 100, true)]
    [InlineData("c_TipoFactor", 6, 50, false)]
    [InlineData("c_PatenteAduanal", 4, 50, false)]
    [InlineData("c_MotivoCancelacion", 2, 100, true)]
    [InlineData("c_TipoCadenaPago", 2, 50, true)]
    public void Model_Built_MapsSimpleCatalog(string tableName, int keyLength, int descriptionLength, bool descriptionRequired)
    {
        using SatDbContext context = CreateContext();
        IModel designTimeModel = context.GetService<IDesignTimeModel>().Model;
        ITable table = Assert.IsAssignableFrom<ITable>(designTimeModel.GetRelationalModel().FindTable(tableName, "sat"));
        Assert.Equal(4, table.Columns.Count());
        IPrimaryKeyConstraint primaryKey = Assert.IsAssignableFrom<IPrimaryKeyConstraint>(table.PrimaryKey);
        Assert.Equal("Clave", Assert.Single(primaryKey.Columns).Name);
        IColumn clave = Assert.IsAssignableFrom<IColumn>(table.FindColumn("Clave"));
        Assert.Equal("nvarchar(" + keyLength + ")", clave.StoreType);
        IColumn descripcion = Assert.IsAssignableFrom<IColumn>(table.FindColumn("Descripcion"));
        Assert.Equal("nvarchar(" + descriptionLength + ")", descripcion.StoreType);
        Assert.Equal(!descriptionRequired, descripcion.IsNullable);
        Assert.Equal("Latin1_General_100_CI_AI", descripcion.Collation);
        IColumn inicio = Assert.IsAssignableFrom<IColumn>(table.FindColumn("FechaInicioVigencia"));
        Assert.Equal("date", inicio.StoreType);
        Assert.True(inicio.IsNullable);
        IColumn fin = Assert.IsAssignableFrom<IColumn>(table.FindColumn("FechaFinVigencia"));
        Assert.Equal("date", fin.StoreType);
        Assert.True(fin.IsNullable);
    }

    [Fact]
    public void Model_Built_HasNoChangesPendingForAMigration()
    {
        using SatDbContext context = CreateContext();
        Assert.False(context.Database.HasPendingModelChanges());
    }

    private static SatDbContext CreateContext()
    {
        DbContextOptionsBuilder<SatDbContext> builder = new DbContextOptionsBuilder<SatDbContext>();
        SatDbContextOptions.Configure(builder, ConnectionString);
        return new SatDbContext(builder.Options);
    }
}
