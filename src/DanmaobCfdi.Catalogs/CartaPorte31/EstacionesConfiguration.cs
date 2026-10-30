using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.CartaPorte31;

internal sealed class EstacionesConfiguration : IEntityTypeConfiguration<Estaciones>
{
    public void Configure(EntityTypeBuilder<Estaciones> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_Estaciones", 12, 100, true);
        builder.Property(entity => entity.ClaveTransporte).HasMaxLength(2);
        builder.Property(entity => entity.Nacionalidad).HasMaxLength(50);
        builder.Property(entity => entity.DesignadorIATA).HasMaxLength(50);
        builder.Property(entity => entity.LineaFerrea).HasMaxLength(50);
    }
}
