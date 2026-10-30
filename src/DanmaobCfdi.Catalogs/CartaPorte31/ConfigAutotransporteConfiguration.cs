using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.CartaPorte31;

internal sealed class ConfigAutotransporteConfiguration : IEntityTypeConfiguration<ConfigAutotransporte>
{
    public void Configure(EntityTypeBuilder<ConfigAutotransporte> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_ConfigAutotransporte", 8, 250, true);
        builder.Property(entity => entity.NumeroLlantas).HasMaxLength(50);
        builder.Property(entity => entity.Remolque).HasMaxLength(50);
    }
}
