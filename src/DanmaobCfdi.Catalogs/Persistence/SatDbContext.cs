using Microsoft.EntityFrameworkCore;

namespace DanmaobCfdi.Catalogs.Persistence;

internal sealed class SatDbContext : DbContext
{
    internal const string Schema = "sat";

    public SatDbContext(DbContextOptions<SatDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SatDbContext).Assembly);
    }
}
