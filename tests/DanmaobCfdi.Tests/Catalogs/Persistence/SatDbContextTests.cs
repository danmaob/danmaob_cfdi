using System;
using System.Linq;
using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;

namespace DanmaobCfdi.Tests.Catalogs.Persistence;

public sealed class SatDbContextTests
{
    private const string ConnectionString = "Server=localhost;Database=DanmaobCfdiModelTests;TrustServerCertificate=True";

    [Fact]
    public void Configure_ConnectionString_UsesHistoryTableInSatSchema()
    {
        DbContextOptionsBuilder<SatDbContext> builder = new DbContextOptionsBuilder<SatDbContext>();
        SatDbContextOptions.Configure(builder, ConnectionString);
        RelationalOptionsExtension extension = RelationalOptionsExtension.Extract(builder.Options);
        Assert.Equal("__EFMigrationsHistory", extension.MigrationsHistoryTableName);
        Assert.Equal("sat", extension.MigrationsHistoryTableSchema);
    }

    [Fact]
    public void Model_Built_MapsCatalogoVersionInSatSchema()
    {
        DbContextOptionsBuilder<SatDbContext> builder = new DbContextOptionsBuilder<SatDbContext>();
        SatDbContextOptions.Configure(builder, ConnectionString);
        using SatDbContext context = new SatDbContext(builder.Options);
        Assert.Equal("sat", context.Model.GetDefaultSchema());
        ITable table = Assert.IsAssignableFrom<ITable>(context.Model.GetRelationalModel().FindTable("CatalogoVersion", "sat"));
        IPrimaryKeyConstraint primaryKey = Assert.IsAssignableFrom<IPrimaryKeyConstraint>(table.PrimaryKey);
        Assert.Equal("Catalogo", Assert.Single(primaryKey.Columns).Name);
        string[] columns = table.Columns.OrderBy(column => column.Name, StringComparer.Ordinal).Select(column => column.Name + ":" + column.StoreType + ":" + column.IsNullable).ToArray();
        string[] expected = new string[] { "Catalogo:nvarchar(50):False", "FechaAplicacionUtc:datetime2:False", "FechaPublicacionSat:date:True", "RevisionSat:nvarchar(20):True", "VersionComponente:nvarchar(50):False", "VersionSat:nvarchar(20):True" };
        Assert.Equal(expected, columns);
    }
}
