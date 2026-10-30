using System.Linq;
using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;

namespace DanmaobCfdi.Tests.Catalogs.Cfdi40;

public sealed class OwnStructureCatalogModelTests
{
    private const string ConnectionString = "Server=localhost;Database=DanmaobCfdiModelTests;TrustServerCertificate=True";

    [Fact]
    public void Model_Built_MapsRegimenFiscal()
    {
        ITable table = FindTable("c_RegimenFiscal");
        Assert.Equal("nvarchar(3)", FindColumn(table, "Clave").StoreType);
        Assert.Equal("bit", FindColumn(table, "AplicaPersonaFisica").StoreType);
        Assert.False(FindColumn(table, "AplicaPersonaFisica").IsNullable);
        Assert.Equal("bit", FindColumn(table, "AplicaPersonaMoral").StoreType);
        Assert.Equal(6, table.Columns.Count());
    }

    [Fact]
    public void Model_Built_MapsUsoCfdi()
    {
        ITable table = FindTable("c_UsoCFDI");
        Assert.Equal("nvarchar(4)", FindColumn(table, "Clave").StoreType);
        Assert.Equal("bit", FindColumn(table, "AplicaPersonaFisica").StoreType);
        Assert.Equal("bit", FindColumn(table, "AplicaPersonaMoral").StoreType);
        Assert.Equal(6, table.Columns.Count());
    }

    [Fact]
    public void Model_Built_MapsUsoCfdiRegimenFiscalReceptor()
    {
        ITable table = FindTable("c_UsoCFDI_RegimenFiscalReceptor");
        IPrimaryKeyConstraint primaryKey = Assert.IsAssignableFrom<IPrimaryKeyConstraint>(table.PrimaryKey);
        Assert.Equal(new[] { "UsoCFDI", "RegimenFiscal" }, primaryKey.Columns.Select(column => column.Name).ToArray());
        Assert.Equal("nvarchar(4)", FindColumn(table, "UsoCFDI").StoreType);
        Assert.Equal("nvarchar(3)", FindColumn(table, "RegimenFiscal").StoreType);
        IForeignKeyConstraint foreignKey = Assert.Single(table.ForeignKeyConstraints);
        Assert.Equal("c_UsoCFDI", foreignKey.PrincipalTable.Name);
        Assert.Equal(2, table.Columns.Count());
    }

    [Fact]
    public void Model_Built_MapsFormaPago()
    {
        ITable table = FindTable("c_FormaPago");
        Assert.Equal("nvarchar(2)", FindColumn(table, "Clave").StoreType);
        Assert.Equal("nvarchar(50)", FindColumn(table, "Bancarizado").StoreType);
        Assert.Equal("nvarchar(100)", FindColumn(table, "PatronCuentaOrdenante").StoreType);
        Assert.Equal("nvarchar(100)", FindColumn(table, "PatronCuentaBeneficiaria").StoreType);
        Assert.Equal("nvarchar(100)", FindColumn(table, "NombreBancoEmisorCuentaOrdenante").StoreType);
        Assert.False(FindColumn(table, "TipoCadenaPago").IsNullable);
        Assert.Equal(14, table.Columns.Count());
    }

    [Fact]
    public void Model_Built_MapsMoneda()
    {
        ITable table = FindTable("c_Moneda");
        Assert.Equal("nvarchar(3)", FindColumn(table, "Clave").StoreType);
        Assert.Equal("int", FindColumn(table, "Decimales").StoreType);
        Assert.False(FindColumn(table, "Decimales").IsNullable);
        Assert.Equal("decimal(5,2)", FindColumn(table, "PorcentajeVariacion").StoreType);
        Assert.True(FindColumn(table, "PorcentajeVariacion").IsNullable);
        Assert.Equal(6, table.Columns.Count());
    }

    [Fact]
    public void Model_Built_MapsImpuesto()
    {
        ITable table = FindTable("c_Impuesto");
        Assert.Equal("nvarchar(3)", FindColumn(table, "Clave").StoreType);
        Assert.Equal("bit", FindColumn(table, "Retencion").StoreType);
        Assert.Equal("bit", FindColumn(table, "Traslado").StoreType);
        Assert.Equal("nvarchar(50)", FindColumn(table, "LocalOFederal").StoreType);
        Assert.Equal(7, table.Columns.Count());
    }

    [Fact]
    public void Model_Built_MapsTipoDeComprobante()
    {
        ITable table = FindTable("c_TipoDeComprobante");
        Assert.Equal("nvarchar(1)", FindColumn(table, "Clave").StoreType);
        Assert.Equal("decimal(24,6)", FindColumn(table, "ValorMaximo").StoreType);
        Assert.False(FindColumn(table, "ValorMaximo").IsNullable);
        Assert.Equal(5, table.Columns.Count());
    }

    [Fact]
    public void Model_Built_MapsTasaOCuota()
    {
        ITable table = FindTable("c_TasaOCuota");
        IPrimaryKeyConstraint primaryKey = Assert.IsAssignableFrom<IPrimaryKeyConstraint>(table.PrimaryKey);
        Assert.Equal("Id", Assert.Single(primaryKey.Columns).Name);
        Assert.Equal("int", FindColumn(table, "Id").StoreType);
        Assert.Equal("decimal(18,6)", FindColumn(table, "ValorMinimo").StoreType);
        Assert.True(FindColumn(table, "ValorMinimo").IsNullable);
        Assert.Equal("decimal(18,6)", FindColumn(table, "ValorMaximo").StoreType);
        Assert.Equal("date", FindColumn(table, "FechaInicioVigencia").StoreType);
        ITableIndex index = Assert.Single(table.Indexes);
        Assert.True(index.IsUnique);
        Assert.Equal(5, index.Columns.Count);
        Assert.Null(index.Filter);
        Assert.Equal(10, table.Columns.Count());
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

