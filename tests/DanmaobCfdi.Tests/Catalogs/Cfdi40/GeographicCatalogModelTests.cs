using System.Linq;
using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;

namespace DanmaobCfdi.Tests.Catalogs.Cfdi40;

public sealed class GeographicCatalogModelTests
{
    private const string ConnectionString = "Server=localhost;Database=DanmaobCfdiModelTests;TrustServerCertificate=True";

    [Fact]
    public void Model_Built_MapsPais()
    {
        ITable table = FindTable("c_Pais");
        Assert.Equal("nvarchar(3)", FindColumn(table, "Clave").StoreType);
        Assert.Equal("nvarchar(100)", FindColumn(table, "Descripcion").StoreType);
        Assert.Equal("nvarchar(100)", FindColumn(table, "FormatoRegistroIdentidadTributaria").StoreType);
        Assert.True(FindColumn(table, "FormatoRegistroIdentidadTributaria").IsNullable);
        Assert.Equal("nvarchar(50)", FindColumn(table, "Agrupaciones").StoreType);
        Assert.Equal(8, table.Columns.Count());
    }

    [Fact]
    public void Model_Built_MapsEstadoWithCompositeKey()
    {
        ITable table = FindTable("c_Estado");
        IPrimaryKeyConstraint primaryKey = Assert.IsAssignableFrom<IPrimaryKeyConstraint>(table.PrimaryKey);
        Assert.Equal(new[] { "Clave", "Pais" }, primaryKey.Columns.Select(column => column.Name).ToArray());
        Assert.Single(table.UniqueConstraints);
        Assert.Equal("nvarchar(3)", FindColumn(table, "Pais").StoreType);
        Assert.Equal("nvarchar(50)", FindColumn(table, "Descripcion").StoreType);
        Assert.Equal(5, table.Columns.Count());
    }

    [Fact]
    public void Model_Built_MapsMunicipioWithCompositeKey()
    {
        ITable table = FindTable("c_Municipio");
        IPrimaryKeyConstraint primaryKey = Assert.IsAssignableFrom<IPrimaryKeyConstraint>(table.PrimaryKey);
        Assert.Equal(new[] { "Clave", "Estado" }, primaryKey.Columns.Select(column => column.Name).ToArray());
        Assert.Single(table.UniqueConstraints);
        Assert.Equal("nvarchar(100)", FindColumn(table, "Descripcion").StoreType);
        Assert.Equal(5, table.Columns.Count());
    }

    [Fact]
    public void Model_Built_MapsLocalidadWithCompositeKey()
    {
        ITable table = FindTable("c_Localidad");
        IPrimaryKeyConstraint primaryKey = Assert.IsAssignableFrom<IPrimaryKeyConstraint>(table.PrimaryKey);
        Assert.Equal(new[] { "Clave", "Estado" }, primaryKey.Columns.Select(column => column.Name).ToArray());
        Assert.Single(table.UniqueConstraints);
        Assert.Equal("nvarchar(2)", FindColumn(table, "Clave").StoreType);
        Assert.Equal(5, table.Columns.Count());
    }

    [Fact]
    public void Model_Built_MapsColoniaWithCompositeKey()
    {
        ITable table = FindTable("c_Colonia");
        IPrimaryKeyConstraint primaryKey = Assert.IsAssignableFrom<IPrimaryKeyConstraint>(table.PrimaryKey);
        Assert.Equal(new[] { "Clave", "CodigoPostal" }, primaryKey.Columns.Select(column => column.Name).ToArray());
        Assert.Single(table.UniqueConstraints);
        Assert.Equal("nvarchar(5)", FindColumn(table, "CodigoPostal").StoreType);
        Assert.Equal("Latin1_General_100_CI_AI", FindColumn(table, "Descripcion").Collation);
        Assert.Equal(5, table.Columns.Count());
    }

    [Fact]
    public void Model_Built_MapsCodigoPostal()
    {
        ITable table = FindTable("c_CodigoPostal");
        Assert.Equal("nvarchar(5)", FindColumn(table, "Clave").StoreType);
        Assert.True(FindColumn(table, "Descripcion").IsNullable);
        Assert.True(FindColumn(table, "Municipio").IsNullable);
        Assert.Equal("tinyint", FindColumn(table, "EstimuloFranjaFronteriza").StoreType);
        Assert.Equal("nvarchar(5)", FindColumn(table, "HoraInicioHorarioVerano").StoreType);
        Assert.Equal("int", FindColumn(table, "DiferenciaHorariaInvierno").StoreType);
        Assert.Equal(17, table.Columns.Count());
    }

    [Fact]
    public void Model_Built_MapsClaveProdServ()
    {
        ITable table = FindTable("c_ClaveProdServ");
        Assert.Equal("nvarchar(8)", FindColumn(table, "Clave").StoreType);
        Assert.Equal("nvarchar(250)", FindColumn(table, "Descripcion").StoreType);
        Assert.Equal("nvarchar(1000)", FindColumn(table, "PalabrasSimilares").StoreType);
        Assert.True(FindColumn(table, "PalabrasSimilares").IsNullable);
        Assert.Equal("Latin1_General_100_CI_AI", FindColumn(table, "PalabrasSimilares").Collation);
        Assert.Equal("tinyint", FindColumn(table, "EstimuloFranjaFronteriza").StoreType);
        Assert.Equal(9, table.Columns.Count());
    }

    [Fact]
    public void Model_Built_MapsClaveUnidad()
    {
        ITable table = FindTable("c_ClaveUnidad");
        Assert.Equal("nvarchar(250)", FindColumn(table, "Descripcion").StoreType);
        Assert.Equal("nvarchar(1000)", FindColumn(table, "DescripcionUnidad").StoreType);
        Assert.True(FindColumn(table, "DescripcionUnidad").IsNullable);
        Assert.Equal("nvarchar(50)", FindColumn(table, "Simbolo").StoreType);
        Assert.Equal(7, table.Columns.Count());
    }

    [Fact]
    public void Model_Built_MapsNumPedimentoAduana()
    {
        ITable table = FindTable("c_NumPedimentoAduana");
        IPrimaryKeyConstraint primaryKey = Assert.IsAssignableFrom<IPrimaryKeyConstraint>(table.PrimaryKey);
        Assert.Equal("Id", Assert.Single(primaryKey.Columns).Name);
        Assert.Equal("nvarchar(2)", FindColumn(table, "Aduana").StoreType);
        Assert.Equal("int", FindColumn(table, "Ejercicio").StoreType);
        ITableIndex index = Assert.Single(table.Indexes);
        Assert.True(index.IsUnique);
        Assert.Equal(3, index.Columns.Count);
        Assert.Equal(7, table.Columns.Count());
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
