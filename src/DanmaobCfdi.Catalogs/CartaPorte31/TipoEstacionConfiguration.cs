using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.CartaPorte31;

internal sealed class TipoEstacionConfiguration : IEntityTypeConfiguration<TipoEstacion>
{
    public void Configure(EntityTypeBuilder<TipoEstacion> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_TipoEstacion", 2, 50, true);
        builder.Property(entity => entity.ClaveTransporte).HasMaxLength(50);
    }
}
