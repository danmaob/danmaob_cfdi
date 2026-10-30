using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.CartaPorte31;

internal sealed class ClaveUnidadPesoConfiguration : IEntityTypeConfiguration<ClaveUnidadPeso>
{
    public void Configure(EntityTypeBuilder<ClaveUnidadPeso> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_ClaveUnidadPeso", 3, 250, true);
        builder.Property(entity => entity.DescripcionUnidad).HasMaxLength(1000);
        builder.Property(entity => entity.Nota).HasMaxLength(250);
        builder.Property(entity => entity.Simbolo).HasMaxLength(50);
        builder.Property(entity => entity.Bandera).HasMaxLength(50);
    }
}
