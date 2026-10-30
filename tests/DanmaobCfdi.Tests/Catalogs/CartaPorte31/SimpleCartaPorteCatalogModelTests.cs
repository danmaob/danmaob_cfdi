using System.Linq;
using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;

namespace DanmaobCfdi.Tests.Catalogs.CartaPorte31;

public sealed class SimpleCartaPorteCatalogModelTests
{
    private const string ConnectionString = "Server=localhost;Database=DanmaobCfdiModelTests;TrustServerCertificate=True";

    [Theory]
    [InlineData("c_CveTransporte", 2, 50, true)]
    [InlineData("c_TipoEmbalaje", 4, 250, true)]
    [InlineData("c_SectorCOFEPRIS", 2, 50, true)]
    [InlineData("c_FormaFarmaceutica", 2, 50, true)]
    [InlineData("c_CondicionesEspeciales", 2, 50, true)]
    [InlineData("c_TipoMateria", 2, 50, true)]
    [InlineData("c_DocumentoAduanero", 2, 250, true)]
    [InlineData("c_ParteTransporte", 4, 50, true)]
    [InlineData("c_FiguraTransporte", 2, 50, true)]
    [InlineData("c_RegistroISTMO", 2, 50, true)]
    [InlineData("c_ConfigMaritima", 3, 50, true)]
    [InlineData("c_ClaveTipoCarga", 3, 50, true)]
    [InlineData("c_ContenedorMaritimo", 5, 50, true)]
    [InlineData("c_TipoDeTrafico", 4, 50, true)]
    [InlineData("c_SubTipoRem", 6, 50, true)]
    [InlineData("c_NumAutorizacionNaviero", 15, 50, false)]
    public void Model_Built_MapsSimpleCartaPorteCatalog(string tableName, int keyLength, int descriptionLength, bool descriptionRequired)
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

    private static SatDbContext CreateContext()
    {
        DbContextOptionsBuilder<SatDbContext> builder = new DbContextOptionsBuilder<SatDbContext>();
        SatDbContextOptions.Configure(builder, ConnectionString);
        return new SatDbContext(builder.Options);
    }
}
