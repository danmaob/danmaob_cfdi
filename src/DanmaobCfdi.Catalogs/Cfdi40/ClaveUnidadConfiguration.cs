using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.Cfdi40;

internal sealed class ClaveUnidadConfiguration : IEntityTypeConfiguration<ClaveUnidad>
{
    public void Configure(EntityTypeBuilder<ClaveUnidad> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_ClaveUnidad", 3, 250, true);
        builder.Property(entity => entity.DescripcionUnidad).HasMaxLength(1000);
        builder.Property(entity => entity.Nota).HasMaxLength(250);
        builder.Property(entity => entity.Simbolo).HasMaxLength(50);
    }
}
