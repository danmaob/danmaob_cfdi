using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.CartaPorte31;

internal sealed class TipoMateriaConfiguration : IEntityTypeConfiguration<TipoMateria>
{
    public void Configure(EntityTypeBuilder<TipoMateria> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_TipoMateria", 2, 50, true);
    }
}
