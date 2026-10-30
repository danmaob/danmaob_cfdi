using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.CartaPorte31;

internal sealed class ClaveProdServCPConfiguration : IEntityTypeConfiguration<ClaveProdServCP>
{
    public void Configure(EntityTypeBuilder<ClaveProdServCP> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_ClaveProdServCP", 8, 250, true);
        builder.Property(entity => entity.PalabrasSimilares).HasMaxLength(1000).UseCollation(SatCatalogConfiguration.DescriptionCollation);
        builder.Property(entity => entity.MaterialPeligroso).HasMaxLength(50);
    }
}
