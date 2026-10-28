using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.Cfdi40;

internal sealed class PatenteAduanalConfiguration : IEntityTypeConfiguration<PatenteAduanal>
{
    public void Configure(EntityTypeBuilder<PatenteAduanal> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_PatenteAduanal", 4, 50, false);
    }
}
