using System.Linq;
using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;

namespace DanmaobCfdi.Tests.Catalogs.CartaPorte31;

public sealed class OwnStructureCartaPorteCatalogModelTests
{
    private const string ConnectionString = "Server=localhost;Database=DanmaobCfdiModelTests;TrustServerCertificate=True";

    [Fact]
    public void Model_Built_MapsClaveProdServCp()
    {
        ITable table = FindTable("c_ClaveProdServCP");
        Assert.Equal("nvarchar(8)", FindColumn(table, "Clave").StoreType);
        Assert.Equal("nvarchar(250)", FindColumn(table, "Descripcion").StoreType);
        Assert.Equal("nvarchar(1000)", FindColumn(table, "PalabrasSimilares").StoreType);
        Assert.Equal("Latin1_General_100_CI_AI", FindColumn(table, "PalabrasSimilares").Collation);
        Assert.Equal("nvarchar(50)", FindColumn(table, "MaterialPeligroso").StoreType);
        Assert.Equal(6, table.Columns.Count());
    }

    [Fact]
    public void Model_Built_MapsMaterialPeligrosoWithNumericKey()
    {
        ITable table = FindTable("c_MaterialPeligroso");
        IPrimaryKeyConstraint primaryKey = Assert.IsAssignableFrom<IPrimaryKeyConstraint>(table.PrimaryKey);
        Assert.Equal("Id", Assert.Single(primaryKey.Columns).Name);
        Assert.Single(table.UniqueConstraints);
        ITableIndex index = Assert.Single(table.Indexes);
        Assert.False(index.IsUnique);
        Assert.Equal("Clave", Assert.Single(index.Columns).Name);
        Assert.Equal("nvarchar(5)", FindColumn(table, "Clave").StoreType);
        Assert.Equal("nvarchar(500)", FindColumn(table, "Descripcion").StoreType);
        Assert.True(FindColumn(table, "ClaseODivision").IsNullable);
        Assert.Equal("nvarchar(250)", FindColumn(table, "NombreTecnico").StoreType);
        Assert.Equal(8, table.Columns.Count());
    }

    [Fact]
    public void Model_Built_MapsClaveUnidadPeso()
    {
        ITable table = FindTable("c_ClaveUnidadPeso");
        Assert.Equal("nvarchar(250)", FindColumn(table, "Descripcion").StoreType);
        Assert.Equal("nvarchar(1000)", FindColumn(table, "DescripcionUnidad").StoreType);
        Assert.True(FindColumn(table, "Simbolo").IsNullable);
        Assert.Equal("nvarchar(50)", FindColumn(table, "Bandera").StoreType);
        Assert.False(FindColumn(table, "Bandera").IsNullable);
        Assert.Equal(8, table.Columns.Count());
    }

    [Fact]
    public void Model_Built_MapsEstaciones()
    {
        ITable table = FindTable("c_Estaciones");
        Assert.Equal("nvarchar(12)", FindColumn(table, "Clave").StoreType);
        Assert.Equal("nvarchar(100)", FindColumn(table, "Descripcion").StoreType);
        Assert.Equal("nvarchar(2)", FindColumn(table, "ClaveTransporte").StoreType);
        Assert.True(FindColumn(table, "DesignadorIATA").IsNullable);
        Assert.True(FindColumn(table, "LineaFerrea").IsNullable);
        Assert.Equal(8, table.Columns.Count());
    }

    [Fact]
    public void Model_Built_MapsTipoEstacion()
    {
        ITable table = FindTable("c_TipoEstacion");
        Assert.Equal("nvarchar(2)", FindColumn(table, "Clave").StoreType);
        Assert.Equal("nvarchar(50)", FindColumn(table, "ClaveTransporte").StoreType);
        Assert.Equal(5, table.Columns.Count());
    }

    [Fact]
    public void Model_Built_MapsRegimenAduanero()
    {
        ITable table = FindTable("c_RegimenAduanero");
        Assert.Equal("nvarchar(3)", FindColumn(table, "Clave").StoreType);
        Assert.Equal("nvarchar(250)", FindColumn(table, "Descripcion").StoreType);
        Assert.Equal("nvarchar(50)", FindColumn(table, "ImpoExpo").StoreType);
        Assert.Equal(5, table.Columns.Count());
    }

    [Fact]
    public void Model_Built_MapsTipoPermiso()
    {
        ITable table = FindTable("c_TipoPermiso");
        Assert.Equal("nvarchar(6)", FindColumn(table, "Clave").StoreType);
        Assert.Equal("nvarchar(50)", FindColumn(table, "ClaveTransporte").StoreType);
        Assert.Equal(5, table.Columns.Count());
    }

    [Fact]
    public void Model_Built_MapsConfigAutotransporte()
    {
        ITable table = FindTable("c_ConfigAutotransporte");
        Assert.Equal("nvarchar(8)", FindColumn(table, "Clave").StoreType);
        Assert.Equal("int", FindColumn(table, "NumeroEjes").StoreType);
        Assert.True(FindColumn(table, "NumeroEjes").IsNullable);
        Assert.Equal("nvarchar(50)", FindColumn(table, "NumeroLlantas").StoreType);
        Assert.True(FindColumn(table, "NumeroLlantas").IsNullable);
        Assert.False(FindColumn(table, "Remolque").IsNullable);
        Assert.Equal(7, table.Columns.Count());
    }

    [Fact]
    public void Model_Built_MapsCodigoTransporteAereo()
    {
        ITable table = FindTable("c_CodigoTransporteAereo");
        Assert.Equal("nvarchar(5)", FindColumn(table, "Clave").StoreType);
        Assert.Equal("nvarchar(100)", FindColumn(table, "Descripcion").StoreType);
        Assert.Equal("nvarchar(50)", FindColumn(table, "DesignadorOACI").StoreType);
        Assert.Equal(6, table.Columns.Count());
    }

    [Fact]
    public void Model_Built_MapsTipoDeServicio()
    {
        ITable table = FindTable("c_TipoDeServicio");
        Assert.Equal("nvarchar(4)", FindColumn(table, "Clave").StoreType);
        Assert.Equal("bit", FindColumn(table, "Contenedor").StoreType);
        Assert.Equal(5, table.Columns.Count());
    }

    [Fact]
    public void Model_Built_MapsTipoCarro()
    {
        ITable table = FindTable("c_TipoCarro");
        Assert.Equal("nvarchar(4)", FindColumn(table, "Clave").StoreType);
        Assert.Equal("bit", FindColumn(table, "Contenedor").StoreType);
        Assert.Equal(5, table.Columns.Count());
    }

    [Fact]
    public void Model_Built_MapsContenedor()
    {
        ITable table = FindTable("c_Contenedor");
        Assert.Equal("nvarchar(4)", FindColumn(table, "Clave").StoreType);
        Assert.Equal("nvarchar(50)", FindColumn(table, "TipoContenedor").StoreType);
        Assert.Equal(5, table.Columns.Count());
    }

    [Fact]
    public void Model_Built_MapsDerechosDePaso()
    {
        ITable table = FindTable("c_DerechosDePaso");
        Assert.Equal("nvarchar(6)", FindColumn(table, "Clave").StoreType);
        Assert.Equal("nvarchar(250)", FindColumn(table, "Entre").StoreType);
        Assert.Equal("nvarchar(250)", FindColumn(table, "Concesionario").StoreType);
        Assert.Equal("nvarchar(50)", FindColumn(table, "OtorgaRecibe").StoreType);
        Assert.Equal(8, table.Columns.Count());
    }

    [Fact]
    public void Model_Built_MapsFraccionArancelaria()
    {
        ITable table = FindTable("c_FraccionArancelaria");
        Assert.Equal("nvarchar(10)", FindColumn(table, "Clave").StoreType);
        Assert.Equal("nvarchar(2000)", FindColumn(table, "Descripcion").StoreType);
        Assert.Equal("Latin1_General_100_CI_AI", FindColumn(table, "Descripcion").Collation);
        Assert.Equal("nvarchar(2)", FindColumn(table, "UMT").StoreType);
        Assert.Equal(5, table.Columns.Count());
    }

    private static ITable FindTable(string tableName)
    {
        DbContextOptionsBuilder<SatDbContext> builder = new DbContextOptionsBuilder<SatDbContext>();
        SatDbContextOptions.Configure(builder, ConnectionString);
        using SatDbContext context = new SatDbContext(builder.Options);
        IModel designTimeModel = context.GetService<IDesignTimeModel>().Model;
        return Assert.IsAssignableFrom<ITable>(designTimeModel.GetRelationalModel().FindTable(tableName, "sat"));
    }

    private static IColumn FindColumn(ITable table, string columnName)
    {
        return Assert.IsAssignableFrom<IColumn>(table.FindColumn(columnName));
    }
}
