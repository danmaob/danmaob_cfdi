using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DanmaobCfdi.Catalogs.Persistence;

internal sealed class SatDbContextFactory : IDesignTimeDbContextFactory<SatDbContext>
{
    internal const string ConnectionVariable = "DANMAOB_CFDI_SAT_CONNECTION";

    public SatDbContext CreateDbContext(string[] args)
    {
        string? connectionString = Environment.GetEnvironmentVariable(ConnectionVariable);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Set the environment variable " + ConnectionVariable + " in the same command that runs dotnet ef.");
        }
        DbContextOptionsBuilder<SatDbContext> builder = new DbContextOptionsBuilder<SatDbContext>();
        SatDbContextOptions.Configure(builder, connectionString);
        return new SatDbContext(builder.Options);
    }
}
