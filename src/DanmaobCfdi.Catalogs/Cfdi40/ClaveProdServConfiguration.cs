using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.Cfdi40;

internal sealed class ClaveProdServConfiguration : IEntityTypeConfiguration<ClaveProdServ>
{
    public void Configure(EntityTypeBuilder<ClaveProdServ> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_ClaveProdServ", 8, 250, true);
        builder.Property(entity => entity.IncluirIvaTrasladado).HasMaxLength(50);
        builder.Property(entity => entity.IncluirIepsTrasladado).HasMaxLength(50);
        builder.Property(entity => entity.ComplementoQueDebeIncluir).HasMaxLength(100);
        builder.Property(entity => entity.PalabrasSimilares).HasMaxLength(1000).UseCollation(SatCatalogConfiguration.DescriptionCollation);
    }
}
