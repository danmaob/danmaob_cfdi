using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.CartaPorte31;

internal sealed class TipoPermisoConfiguration : IEntityTypeConfiguration<TipoPermiso>
{
    public void Configure(EntityTypeBuilder<TipoPermiso> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_TipoPermiso", 6, 250, true);
        builder.Property(entity => entity.ClaveTransporte).HasMaxLength(50);
    }
}
