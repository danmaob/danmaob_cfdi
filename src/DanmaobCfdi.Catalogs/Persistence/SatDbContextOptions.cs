using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DanmaobCfdi.Catalogs.Persistence;

internal static class SatDbContextOptions
{
    internal const string HistoryTable = "__EFMigrationsHistory";

    internal static void Configure(DbContextOptionsBuilder builder, string connectionString)
    {
        builder.UseSqlServer(connectionString, ConfigureSqlServer);
    }

    private static void ConfigureSqlServer(SqlServerDbContextOptionsBuilder sqlServer)
    {
        sqlServer.MigrationsHistoryTable(HistoryTable, SatDbContext.Schema);
    }
}
